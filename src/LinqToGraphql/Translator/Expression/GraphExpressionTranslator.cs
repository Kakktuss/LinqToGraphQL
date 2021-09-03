using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqToGraphQL.Extensions;
using LinqToGraphQL.Translator.Details;

namespace LinqToGraphQL.Translator.Expression
{
	public class GraphExpressionTranslator
	{
		
		private readonly List<IncludeDetail> _includeTree;

		public GraphExpressionTranslator()
		{
			_includeTree = new List<IncludeDetail>();
		}

		internal List<IncludeDetail> Translate(System.Linq.Expressions.Expression expression)
		{
			Visit(expression, "");
			return _includeTree;
		}

		protected Tuple<System.Linq.Expressions.Expression, string> Visit(System.Linq.Expressions.Expression? node, string parent = "")
		{
			if (node is MethodCallExpression methodCallExpression)
			{
				return VisitMethodCall(methodCallExpression, parent);
			} else if (node is MemberExpression memberExpression)
			{
				return VisitMember(memberExpression, parent);
			} 
			else if (node is LambdaExpression lambdaExpression)
			{
				return Visit(lambdaExpression.Body, parent);
			} else if (node is MemberInitExpression memberInitExpression)
			{
				return VisitMemberInit(memberInitExpression, parent);
			}
			
			return new Tuple<System.Linq.Expressions.Expression, string>(node, parent);
		}

		protected Tuple<System.Linq.Expressions.Expression, string> VisitMemberInit(MemberInitExpression node, string parent)
		{
			if (node.Bindings.Any())
			{
				IncludeDetail parentInclude = null;

				if (string.IsNullOrEmpty(parent))
				{
					foreach (var binding in node.Bindings)
					{
						if (!_includeTree.Exists(e => e.Name == binding.Member.Name))
						{
							_includeTree.Add(new IncludeDetail(binding.Member.Name, binding.Member));
						}
					}
				} else
				{
					var parentNames = parent.Split(".");

					parentInclude = _includeTree.FirstOrDefault(e => e.Name == parentNames.First());

					foreach (var parentName in parentNames.Skip(1))
					{
						parentInclude = parentInclude?.Includes.FirstOrDefault(e => e.Name == parentName);
					}

					if (parentInclude is not null)
					{
						foreach (var binding in node.Bindings)
						{
							if (!parentInclude.Includes.Exists(e => e.Name == binding.Member.Name))
							{
								parentInclude.AddSubInclude(new IncludeDetail(binding.Member.Name, binding.Member));
							}
						}
					}
				}
			}
			
			return new Tuple<System.Linq.Expressions.Expression, string>(node, parent);
		}
		
		protected Tuple<System.Linq.Expressions.Expression, string> VisitMember(MemberExpression node, string parent)
		{
			if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
			{
				IncludeDetail parentInclude = null;

				if (string.IsNullOrEmpty(parent))
				{
					if (!_includeTree.Exists(e => e.Name == node.Member.Name))
					{
						parentInclude = new IncludeDetail(node.Member.Name, node.Member);
					
						_includeTree.Add(parentInclude);
					}
				}
				else
				{
					var parentNames = parent.Split(".");

					parentInclude = _includeTree.FirstOrDefault(e => e.Name == parentNames.First());

					foreach (var parentName in parentNames.Skip(1))
					{
						parentInclude = parentInclude?.Includes.FirstOrDefault(e => e.Name == parentName);
					}

					if (parentInclude is not null)
					{
						if (!parentInclude.Includes.Exists(e => e.Name == node.Member.Name))
						{
							parentInclude.AddSubInclude(new IncludeDetail(node.Member.Name, node.Member));
						}
					}
				}
			}
			
			return new Tuple<System.Linq.Expressions.Expression, string>(node, parent);
		}

		protected Tuple<System.Linq.Expressions.Expression, string> VisitMethodCall(MethodCallExpression node, string parent)
		{
			if (node.Method.DeclaringType == typeof(GraphQueryableExtensions) && node.Method.Name == "Include")
			{
				if (node.Arguments[0] is MethodCallExpression methodCallExpression)
				{
					Visit(methodCallExpression, parent);
				}
				
				return Visit(StripQuotes(node.Arguments[1]), parent);
			} 
			else if (node.Method.DeclaringType == typeof(GraphQueryableExtensions) && node.Method.Name == "ThenInclude")
			{
				if (node.Arguments[0] is MethodCallExpression methodCallExpression)
				{
					var resultingNode = Visit(methodCallExpression, parent);

					if (resultingNode.Item1 is MemberExpression resultingMemberExpression)
					{
						parent += $"{(!string.IsNullOrEmpty(resultingNode.Item2) ? $"{resultingNode.Item2}." : "")}{resultingMemberExpression.Member.Name}";
					} else if (resultingNode.Item1 is MethodCallExpression resultingMethodCallExpression)
					{
						parent += $"{(!string.IsNullOrEmpty(resultingNode.Item2) ? $"{resultingNode.Item2}." : "")}{resultingMethodCallExpression.Method.Name}";
					} else
					{
						parent += !string.IsNullOrEmpty(resultingNode.Item2) ? $"{resultingNode.Item2}" : "";
					}
				}
				
				return Visit(StripQuotes(node.Arguments[1]), parent);
			} else if (node.Method.Name == "Select")
			{
				if (node.Arguments[0] is MethodCallExpression methodCallExpression)
				{
					var resultingNode = Visit(methodCallExpression, parent);
					
					if (resultingNode.Item1 is MemberExpression resultingMemberExpression)
					{
						parent += $"{(!string.IsNullOrEmpty(resultingNode.Item2) ? $"{resultingNode.Item2}." : "")}{resultingMemberExpression.Member.Name}";
					} else if (resultingNode.Item1 is MethodCallExpression resultingMethodCallExpression)
					{
						parent += $"{(!string.IsNullOrEmpty(resultingNode.Item2) ? $"{resultingNode.Item2}." : "")}{resultingMethodCallExpression.Method.Name}";
					} else
					{
						parent += !string.IsNullOrEmpty(resultingNode.Item2) ? $"{resultingNode.Item2}" : "";
					}
				}

				return Visit(StripQuotes(node.Arguments[1]), parent);
			}
			else
			{
				IncludeDetail parentInclude = null;

				var zippedMethodParameters = node.Method.GetParameters()
					.Zip(node.Arguments,
						(parameter, value) => new Tuple<ParameterInfo, System.Linq.Expressions.Expression>(parameter, value));
				
				if (string.IsNullOrEmpty(parent))
				{
					if (!_includeTree.Exists(e => e.Name == node.Method.Name))
					{
						parentInclude = new IncludeDetail(node.Method.Name, node.Method);

						foreach ((var methodParameter, var methodParameterValue) in zippedMethodParameters)
						{
							if (methodParameterValue is ConstantExpression methodParameterValueConstantExpression)
							{
								parentInclude.AddInput(new InputDetail($"{node.Method.Name}{char.ToUpper(methodParameter.Name[0])}{methodParameter.Name[1..]}".ToCamel(), methodParameter.ParameterType, methodParameter.Name, methodParameterValueConstantExpression.Value, methodParameter));
							}
						}
						
						_includeTree.Add(parentInclude);
					}
				}
				else
				{
					var parentNames = parent.Split(".");

					parentInclude = _includeTree.FirstOrDefault(e => e.Name == parentNames.First());

					foreach (var parentName in parentNames.Skip(1))
					{
						parentInclude = parentInclude?.Includes.FirstOrDefault(e => e.Name == parentName);
					}

					if (parentInclude is not null)
					{
						if (!parentInclude.Includes.Exists(e => e.Name == node.Method.Name))
						{
							var subInclude = new IncludeDetail(node.Method.Name, node.Method);
							
							foreach ((var methodParameter, var methodParameterValue) in zippedMethodParameters)
							{
								if (methodParameterValue is ConstantExpression methodParameterValueConstantExpression)
								{
									subInclude.AddInput(new InputDetail(
										$"{string.Concat(parentNames)}{node.Method.Name}{char.ToUpper(methodParameter.Name[0])}{methodParameter.Name[1..]}".ToCamel(), 
										methodParameter.ParameterType,
										methodParameter.Name, 
										methodParameterValueConstantExpression.Value, 
										methodParameter));
								}
							}
								
							parentInclude.AddSubInclude(subInclude);
						}
					}
				}
			}

			return new Tuple<System.Linq.Expressions.Expression, string>(node, parent);
		}
		
		private static System.Linq.Expressions.Expression StripQuotes(System.Linq.Expressions.Expression e)
		{
			while (e.NodeType == ExpressionType.Quote)
			{
				e = ((UnaryExpression) e).Operand;
			}

			return e;
		}

	}
}
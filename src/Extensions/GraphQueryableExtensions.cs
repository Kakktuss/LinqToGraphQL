using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LinqToGraphQL.Provider;
using LinqToGraphQL.Set.Include;

namespace LinqToGraphQL.Extensions
{
	public static class GraphQueryableExtensions
	{
		#region Includable methods

		// Include
		public static IIncludableQueryable<TEntity, TProperty> Include<TEntity, TProperty>(this IQueryable<TEntity> source,
			Expression<Func<TEntity, TProperty>> navigationPropertyPath)
		{
			return new IncludableQueryable<TEntity, TProperty>(source.Provider is GraphQueryProvider
				? source.Provider.CreateQuery<TEntity>(Expression.Call(
					instance: null,
					method: IncludeMethodInfo().MakeGenericMethod(typeof(TEntity), typeof(TProperty)),
					arguments: new[]
					{
						source.Expression, Expression.Quote(navigationPropertyPath)
					}))
				: source);
		}

		internal static MethodInfo IncludeMethodInfo() => typeof(GraphQueryableExtensions)
			.GetTypeInfo()
			.GetDeclaredMethods(nameof(Include))
			.Single(methodInfo =>
			{
				var genericArguments = methodInfo.GetGenericArguments().Count();
				
				return genericArguments == 2
				       && methodInfo.GetParameters().Any(parameterInfo => parameterInfo.Name == "navigationPropertyPath" && parameterInfo.ParameterType != typeof(string));
			});
		
		// ThenInclude enumerable
		public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> source,
			Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
		{
			return new IncludableQueryable<TEntity, TProperty>(source.Provider is GraphQueryProvider ?
				source.Provider.CreateQuery<TEntity>(
					Expression.Call(
						instance: null,
						method: ThenIncludeAfterEnumerableMethodInfo().MakeGenericMethod(typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)),
						arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) })) 
				: source);
		}
		
		internal static MethodInfo ThenIncludeAfterEnumerableMethodInfo() => 
			typeof(GraphQueryableExtensions).GetTypeInfo().GetDeclaredMethods(nameof(ThenInclude))
				.Where(methodInfo => methodInfo.GetGenericArguments().Count() == 3)
				.Single(methodInfo =>
				{
					var typeInfo = methodInfo.GetParameters()[0].ParameterType.GenericTypeArguments[1];
					return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);
				});

		// ThenInclude method
		public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IIncludableQueryable<TEntity, TPreviousProperty> source,
			Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
		{
			return new IncludableQueryable<TEntity, TProperty>(source.Provider is GraphQueryProvider
				? source.Provider.CreateQuery<TEntity>(
					Expression.Call(
						instance: null,
						method: ThenIncludeAfterReferenceMethodInfo().MakeGenericMethod(
							typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)),
						arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) }))
				: source);
		}

		internal static MethodInfo ThenIncludeAfterReferenceMethodInfo() =>  typeof(GraphQueryableExtensions)
			.GetTypeInfo().GetDeclaredMethods(nameof(ThenInclude))
			.Single(methodInfo =>
			{
				return methodInfo.GetGenericArguments().Count() == 3 
				       && methodInfo.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter;
			});

		#endregion
		
		#region Selectable methods

		public static IIncludableQueryable<TEntity, TProperty> Select<TEntity, TPreviousProperty, TProperty>(this IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> source,
			Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
		{
			return new IncludableQueryable<TEntity, TProperty>(source.Provider is GraphQueryProvider ?
				source.Provider.CreateQuery<TEntity>(
					Expression.Call(
						instance: null,
						method: SelectAfterEnumerableMethodInfo().MakeGenericMethod(typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)),
						arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) })) 
				: source);
		}
		
		internal static MethodInfo SelectAfterEnumerableMethodInfo() => 
			typeof(GraphQueryableExtensions).GetTypeInfo().GetDeclaredMethods(nameof(Select))
				.Where(methodInfo => methodInfo.GetGenericArguments().Count() == 3)
				.Single(methodInfo =>
				{
					var typeInfo = methodInfo.GetParameters()[0].ParameterType.GenericTypeArguments[1];
					return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);
				});

		#endregion
		
		#region Aggregate methods

		public static TSource ToItem<TSource>(this IQueryable<TSource> source)
		{
			return source.AsEnumerable().First();
		}

		#endregion

		#region Aggregate async methods

		public static async Task<List<TSource>> ToListAsync<TSource>(
			this IQueryable<TSource> source,
			CancellationToken cancellationToken = default)
		{
			var list = new List<TSource>();
			await foreach (var element in source.AsAsyncEnumerable().WithCancellation(cancellationToken))
			{
				list.Add(element);
			}

			return list;
		}

		public static async Task<TSource> ToItemAsync<TSource>(this IQueryable<TSource> source,
			CancellationToken cancellationToken = default)
			where TSource: class
		{
			await foreach (var element in source.AsAsyncEnumerable().WithCancellation(cancellationToken))
			{
				return element;
			}

			return null;
		}

		#endregion
	}
}
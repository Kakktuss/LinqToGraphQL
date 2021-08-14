using System;

namespace LinqToGraphQL.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class GraphParameterTypeAttribute : Attribute
	{

		public GraphParameterTypeAttribute(Type type)
		{
			Type = type;
		}
		
		public Type Type { get; }
		
	}
}
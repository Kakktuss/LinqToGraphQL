using System;

namespace LinqToGraphQL.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class GraphParameterTypeNameAttribute : Attribute
	{

		public GraphParameterTypeNameAttribute(string typeName)
		{
			TypeName = typeName;
		}
		
		public string TypeName { get; }
		
	}
}
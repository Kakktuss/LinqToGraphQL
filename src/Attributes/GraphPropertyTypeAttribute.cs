using System;

namespace LinqToGraphQL.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class GraphPropertyTypeAttribute : Attribute
	{

		public GraphPropertyTypeAttribute(Type type)
		{
			Type = type;
		}
		
		public Type Type { get; }
		
	}
}
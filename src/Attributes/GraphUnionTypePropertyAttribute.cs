using System;

namespace LinqToGraphQL.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class GraphUnionTypePropertyAttribute : Attribute
	{
		public GraphUnionTypePropertyAttribute(params Type[] unionTypes)
		{
			UnionTypes = unionTypes;
		}
		
		public Type[] UnionTypes { get; set; }
	}
}
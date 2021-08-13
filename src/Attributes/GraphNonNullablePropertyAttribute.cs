using System;

namespace LinqToGraphQL.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class GraphNonNullablePropertyAttribute : Attribute
	{
		
	}
}
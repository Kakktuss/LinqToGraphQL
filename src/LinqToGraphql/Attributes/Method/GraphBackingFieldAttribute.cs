using System;

namespace LinqToGraphQL.Attributes.Method
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class GraphBackingFieldAttribute : Attribute
	{
		public GraphBackingFieldAttribute(string backingField)
		{
			BackingField = backingField;
		}
		
		public string BackingField { get; set; }
	}
}
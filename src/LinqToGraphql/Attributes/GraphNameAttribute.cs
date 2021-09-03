using System;
using System.Text.Json.Serialization;

namespace LinqToGraphQL.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class GraphNameAttribute : JsonAttribute
	{

		public GraphNameAttribute(string name)
		{
			Name = name;
		}
		
		public string Name { get; }
		
	}
}
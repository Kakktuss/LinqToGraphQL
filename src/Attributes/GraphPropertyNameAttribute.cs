using System;
using System.Text.Json.Serialization;

namespace LinqToGraphQL.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class GraphPropertyNameAttribute : JsonAttribute
	{

		public GraphPropertyNameAttribute(string name)
		{
			Name = name;
		}
		
		public string Name { get; }
		
	}
}
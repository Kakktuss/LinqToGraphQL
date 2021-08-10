using System;

namespace Client.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
	public class GraphPropertyNameAttribute : Attribute
	{

		public GraphPropertyNameAttribute(string name)
		{
			Name = name;
		}
		
		public string Name { get; }
		
	}
}
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Client.Set.Configuration
{
	public class GraphSetQueryConfiguration
	{
		public GraphSetQueryConfiguration(GraphSetTypes type)
		{
			Arguments = new();

			Type = type;
		}
		
		internal string Name { get; set; }
		
		internal Dictionary<string, Tuple<ParameterInfo, object>> Arguments { get; set; }
		
		internal GraphSetTypes Type { get; set; }
	}
}
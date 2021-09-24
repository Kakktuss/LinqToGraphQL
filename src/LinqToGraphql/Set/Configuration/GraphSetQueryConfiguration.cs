using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinqToGraphQL.Set.Configuration
{
	public class GraphSetQueryConfiguration
	{
		public GraphSetQueryConfiguration(GraphSetTypes? type = null)
		{
			Arguments = new();

			Type = type ?? GraphSetTypes.Query;
		}
		
		internal string Name { get; set; }
		
		internal Dictionary<string, Tuple<ParameterInfo, object>> Arguments { get; set; }
		
		internal GraphSetTypes Type { get; set; }
	}
}
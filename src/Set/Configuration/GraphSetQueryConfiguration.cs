using System.Collections.Generic;

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
		
		internal Dictionary<string, object> Arguments { get; set; }
		
		internal GraphSetTypes Type { get; set; }
	}
}
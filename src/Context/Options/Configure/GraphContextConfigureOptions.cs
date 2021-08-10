using System;
using System.Collections.Generic;
using Client.Set.Configuration;

namespace Client.Context.Options.Configure
{
	public class GraphContextConfigureOptions
	{
		public GraphContextConfigureOptions(Dictionary<Type, GraphSetConfiguration> configurations)
		{
			Configurations = configurations;
		}
		
		internal Dictionary<Type, GraphSetConfiguration> Configurations;
	}
}
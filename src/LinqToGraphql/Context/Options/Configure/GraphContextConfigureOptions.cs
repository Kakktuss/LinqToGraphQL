using System;
using System.Collections.Generic;
using LinqToGraphQL.Set.Configuration;

namespace LinqToGraphQL.Context.Options.Configure
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
using System;
using System.Collections.Generic;
using LinqToGraphQL.Context.Options.Configure;
using LinqToGraphQL.Set.Configuration;
using LinqToGraphQL.Set.Configuration.Builder;

namespace LinqToGraphQL.Context.Options.Builder.Configure
{
	public class GraphContextConfigureOptionsBuilder
	{

		private readonly Dictionary<Type, GraphSetConfiguration> _configurations;

		public GraphContextConfigureOptionsBuilder()
		{
			_configurations = new();
		}
		
		public GraphContextConfigureOptionsBuilder ConfigureSet<T>(Action<GraphSetConfigurationBuilder> graphSetConfigurationAction)
		{
			var graphSetConfigurationBuilder = new GraphSetConfigurationBuilder();

			graphSetConfigurationAction(graphSetConfigurationBuilder);
			
			if (!_configurations.ContainsKey(typeof(T)))
			{
				_configurations.Add(typeof(T), graphSetConfigurationBuilder.Build());
			}

			return this;
		}

		internal GraphContextConfigureOptions Build()
		{
			return new GraphContextConfigureOptions(_configurations);
		}

	}
}
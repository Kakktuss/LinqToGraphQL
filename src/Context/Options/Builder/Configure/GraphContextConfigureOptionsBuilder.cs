using System;
using System.Collections.Generic;
using Client.Context.Options.Configure;
using Client.Set.Configuration;
using Client.Set.Configuration.Builder;

namespace Client.Context.Options.Builder.Configure
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
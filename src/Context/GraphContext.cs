using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Client.Attributes;
using Client.Client;
using Client.Context.Options.Builder.Configure;
using Client.Context.Options.Configure;
using Client.Provider;
using Client.Set;
using Client.Set.Configuration;
using Client.Set.Configuration.Builder;
using HttpClientFactoryLite;

namespace Client.Context
{
    public class GraphContext
    {
        protected IDictionary<string, object> ContextArguments;
        
        private readonly ClientFactorySingleton _clientFactorySingleton;

        private readonly Dictionary<Type, GraphSetConfiguration> _configurations;

        protected GraphContext(IDictionary<string, object> contextArguments = null)
        {
            ContextArguments = contextArguments;
            
            _clientFactorySingleton = ClientFactorySingleton.Instance;

            _configurations = BuildConfigure().Configurations;
        }
        
        public GraphSet<T> Set<T>(object[] parameterValues = null, Action<GraphSetConfigurationBuilder> graphSetConfigurationAction = null, [CallerMemberName] string queryName = "")
        {
            GraphSetConfiguration graphSetConfiguration = new(default, default, default);
            
            if (_configurations.Any(e => e.Key == typeof(T)))
            {
                if (graphSetConfigurationAction is not null)
                {
                    GraphSetExistingConfigurationBuilder configurationBuilder = new GraphSetExistingConfigurationBuilder(_configurations.First(e => e.Key == typeof(T)).Value);
                    
                    graphSetConfigurationAction(configurationBuilder);

                    graphSetConfiguration = configurationBuilder.Build();
                } else
                {
                    graphSetConfiguration = _configurations.First(e => e.Key == typeof(T)).Value;
                }
            } else
            {
                if (graphSetConfigurationAction is not null)
                {
                    GraphSetConfigurationBuilder configurationBuilder = new GraphSetConfigurationBuilder();

                    graphSetConfigurationAction(configurationBuilder);
                    
                    graphSetConfiguration = configurationBuilder.Build();
                } else
                {
                    throw new ArgumentNullException("graphSetConfigurationAction",
                        "You must at least define one configuration for the current GraphSet. Either build it in Configure method or either using the parameter \"graphSetConfigurationAction\" to build the current query configuration");
                }
            }

            var realQueryName = queryName;
            
            AttributesParserHelper.CheckMethodNameAttributes(ref realQueryName, GetType().GetMethod(queryName));

            graphSetConfiguration.Query.Name = realQueryName;
            
            graphSetConfiguration.Query.Arguments = BuildSetCallerArgumentsDictionnary(parameterValues, queryName);

            return new GraphSet<T>(new GraphQueryProvider(graphSetConfiguration, _clientFactorySingleton.HttpClientFactory.CreateClient("graph")));
        }
        
        private GraphContextConfigureOptions BuildConfigure()
        {
            var graphContextConfigureOptionsBuilder = new GraphContextConfigureOptionsBuilder();
            
            Configure(graphContextConfigureOptionsBuilder);

            return graphContextConfigureOptionsBuilder.Build();
        }

        protected virtual void Configure(GraphContextConfigureOptionsBuilder graphContextConfigureOptionsBuilder) { }

        private Dictionary<string, Tuple<ParameterInfo, object>> BuildSetCallerArgumentsDictionnary(object[] parameterValues, string queryName)
        {
            var parameters = GetType().GetMethod(queryName).GetParameters();
            var arguments = parameters.Zip(parameterValues, (info, value) =>
            {
                return new
                {
                    info.Name,
                    Value = new Tuple<ParameterInfo, object>(info, value)
                };
            }).ToDictionary(arg => arg.Name, arg => arg.Value);
            return arguments;
        }
    }
}
using System;

namespace Client.Set.Configuration.Builder
{
	public class GraphSetConfigurationBuilder
	{
		protected string Url;

		protected GraphSetHttpConfigurationBuilder HttpConfigurationBuilder = new();

		protected GraphSetQueryConfigurationBuilder QueryConfigurationBuilder = new();
		
		public GraphSetConfigurationBuilder WithUrl(string url)
		{
			Url = url;

			HttpConfigurationBuilder.RequestUri = url;

			return this;
		}

		public GraphSetConfigurationBuilder ConfigureHttp(Action<GraphSetHttpConfigurationBuilder> httpConfigurationBuilder)
		{
			httpConfigurationBuilder(HttpConfigurationBuilder);
			
			return this;
		}

		public GraphSetConfigurationBuilder ConfigureQuery(Action<GraphSetQueryConfigurationBuilder> queryConfigurationBuilder)
		{
			queryConfigurationBuilder(QueryConfigurationBuilder);

			return this;
		}
		
		internal virtual GraphSetConfiguration Build()
		{
			return new GraphSetConfiguration(Url, QueryConfigurationBuilder.Build(),HttpConfigurationBuilder.Build());
		}
	}
}
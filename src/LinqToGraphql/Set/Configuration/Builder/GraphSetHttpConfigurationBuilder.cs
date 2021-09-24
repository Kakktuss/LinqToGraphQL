using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LinqToGraphQL.Set.Configuration.Builder
{
	public class GraphSetHttpConfigurationBuilder
	{
		internal string RequestUri { get; set; }

		protected HttpMethod Method { get; set; }

		protected HttpRequestHeaders Headers { get; set; } = new HttpRequestMessage().Headers;

		public GraphSetHttpConfigurationBuilder WithMethod(HttpMethod method)
		{
			Method = method;

			return this;
		}

		public GraphSetHttpConfigurationBuilder AddHeader(string name, string value)
		{
			Headers.Add(name, value);

			return this;
		}

		public GraphSetHttpConfigurationBuilder ConfigureHeaders(Action<HttpRequestHeaders> headersAction)
		{
			headersAction(Headers);

			return this;
		}
		
		internal GraphSetHttpConfiguration Build()
		{
			return new GraphSetHttpConfiguration(RequestUri, Method, Headers);
		}
	}
}
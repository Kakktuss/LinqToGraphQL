using System.Net.Http;
using System.Net.Http.Headers;

namespace LinqToGraphQL.Set.Configuration
{
	#nullable enable
	public class GraphSetHttpConfiguration
	{
		public GraphSetHttpConfiguration(string requestUri, HttpMethod? method = null, HttpRequestHeaders? headers = null)
		{
			RequestUri = requestUri;

			Headers = headers ?? new HttpRequestMessage().Headers;

			Method = method ?? HttpMethod.Get;
		}
		
		internal string RequestUri { get; set; }

		public HttpRequestHeaders Headers { get; set; }
		
		public HttpMethod Method { get; set; }
	}
	#nullable disable
}
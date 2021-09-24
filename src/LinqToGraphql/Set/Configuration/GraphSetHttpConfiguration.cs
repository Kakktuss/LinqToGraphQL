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

			Method = method;
			
			Headers = headers ?? new HttpRequestMessage().Headers;
		}
		
		internal string RequestUri { get; set; }

		public HttpRequestHeaders Headers { get; set; }
		
		public HttpMethod? Method { get; set; }
	}
	#nullable disable
}
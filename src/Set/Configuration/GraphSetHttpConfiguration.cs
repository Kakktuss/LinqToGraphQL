using System.Net.Http;
using System.Net.Http.Headers;

namespace LinqToGraphQL.Set.Configuration
{
	public class GraphSetHttpConfiguration
	{
		public GraphSetHttpConfiguration(string requestUri, HttpMethod method, HttpRequestHeaders headers = null)
		{
			RequestUri = requestUri;

			if (headers is null)
			{
				headers = new HttpRequestMessage().Headers;
			}
			
			Headers = headers;

			if (method is null)
			{
				method = HttpMethod.Get;
			}
			
			Method = method;
		}
		
		internal string RequestUri { get; set; }

		public HttpRequestHeaders Headers { get; set; }
		
		public HttpMethod Method { get; set; }
	}
}
using System.Net.Http;
using System.Net.Http.Headers;

namespace Client.Set.Configuration
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
			
			Headers = new HttpRequestMessage().Headers;

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
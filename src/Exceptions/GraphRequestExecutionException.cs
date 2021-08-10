using System;
using System.Net.Http;

namespace Client
{
	public class GraphRequestExecutionException : Exception
	{
		public GraphRequestExecutionException(HttpResponseMessage message) : base("An error happened while trying to process the http request, see the ResponseMessage variable.")
		{
			ResponseMessage = message;
		}

		public HttpResponseMessage ResponseMessage { get; private set; }
	}
}
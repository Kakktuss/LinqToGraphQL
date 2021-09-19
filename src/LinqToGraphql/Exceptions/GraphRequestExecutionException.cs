using System;
using System.Net.Http;

namespace LinqToGraphQL.Exceptions
{
	public class GraphRequestExecutionException : Exception
	{
		public GraphRequestExecutionException(string query, HttpResponseMessage message) : base("An error happened while trying to process the http request, see the ResponseMessage variable.")
		{
			Query = query;
			
			ResponseMessage = message;
		}
		
		public string Query { get; set; }

		public HttpResponseMessage ResponseMessage { get; private set; }
	}
}
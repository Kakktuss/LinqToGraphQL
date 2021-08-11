using System;

namespace LinqToGraphQL.Set.Configuration
{
	public class GraphSetConfiguration : ICloneable
	{

		public GraphSetConfiguration(string url, GraphSetQueryConfiguration query, GraphSetHttpConfiguration http)
		{
			Url = url;

			Query = query;
			
			Http = http;
		}
		
		public string Url { get; internal set; }
		
		public GraphSetQueryConfiguration Query { get; internal set; }

		public GraphSetHttpConfiguration Http { get; internal set; }

		public object Clone()
		{
			var cloned = (GraphSetConfiguration) this.MemberwiseClone();

			cloned.Query = Query;
			cloned.Http = Http;
			
			return cloned;
		}
	}
}
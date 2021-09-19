using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LinqToGraphQL.Translator.Details
{
	public class QueryDetail
	{
		[JsonProperty("query")]
		public string Query { get; set; }
		
		[JsonProperty("variables")]
		public Dictionary<string, object> Variables { get; set; }
	}
}
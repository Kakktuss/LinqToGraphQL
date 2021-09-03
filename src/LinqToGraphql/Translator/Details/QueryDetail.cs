using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToGraphQL.Translator.Details
{
	public class QueryDetail
	{
		[JsonPropertyName("query")]
		public string Query { get; set; }
		
		[JsonPropertyName("variables")]
		public Dictionary<string, object> Variables { get; set; }
	}
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Client.Translator.Details
{
	public class QueryDetail
	{
		[JsonPropertyName("query")]
		public string Query { get; set; }
		
		[JsonPropertyName("variables")]
		public Dictionary<string, object> Variables { get; set; }
	}
}
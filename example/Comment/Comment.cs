using LinqToGraphQL.Attributes;

namespace TestClient.Comment
{
	public class Comment
	{
		[GraphPropertyName("title")]
		public string Title { get; set; }
		
		[GraphPropertyName("content")]
		public string Content { get; set; }
	}
}
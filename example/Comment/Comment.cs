using LinqToGraphQL.Attributes;

namespace TestClient.Comment
{
	public class Comment
	{
		[GraphName("title")]
		public string Title { get; set; }
		
		[GraphName("content")]
		public string Content { get; set; }
	}
}
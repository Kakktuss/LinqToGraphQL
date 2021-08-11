using System.Collections.Generic;
using LinqToGraphQL.Attributes;

namespace TestClient.Post
{
	public class Post
	{
		[GraphPropertyName("title")]
		public string Title { get; set; }
		
		[GraphPropertyName("content")]
		public string Content { get; set; }

		[GraphPropertyName("comments")]
		public List<Comment.Comment> Comments([GraphPropertyName("commentsId")] int id) => new List<Comment.Comment>();
	}
}
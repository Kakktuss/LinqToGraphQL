using System.Collections.Generic;
using LinqToGraphQL.Attributes;

namespace TestClient.Post
{
	public class Post
	{
		[GraphName("title")]
		public string Title { get; set; }
		
		[GraphName("content")]
		public string Content { get; set; }

		[GraphName("comments")]
		public List<Comment.Comment> Comments([GraphName("commentsId")] int id) => new List<Comment.Comment>();
	}
}
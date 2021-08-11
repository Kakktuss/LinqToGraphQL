using System.Collections.Generic;
using LinqToGraphQL.Attributes;

namespace TestClient.User
{
	public class User
	{
		[GraphPropertyName("name")]
		public string Name { get; set; }
		
		[GraphPropertyName("username")]
		public string Username { get; set; }

		[GraphPropertyName("posts")]
		public List<Post.Post> Posts([GraphNonNullableProperty] [GraphPropertyName("postsId")] int id) => new List<Post.Post>();
		
		[GraphPropertyName("comments")]
		public List<Comment.Comment> Comments { get; set; }
	}
}
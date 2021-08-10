using System.Collections.Generic;
using Client.Attributes;
using Client.Translator.Behavior;

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
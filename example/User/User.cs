using System.Collections.Generic;
using LinqToGraphQL.Attributes;

namespace TestClient.User
{
	public class User
	{
		[GraphName("name")]
		[GraphUnionTypeProperty(typeof(int), typeof(string))]
		public string Name { get; set; }
		
		[GraphName("username")]
		public string Username { get; set; }

		[GraphName("posts")]
		public List<Post.Post> Posts([GraphNonNullableParameter] [GraphName("postsId")] [GraphParameterType(typeof(string))] int id) => new List<Post.Post>();
		
		[GraphName("comments")]
		public List<Comment.Comment> Comments { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToGraphQL.Extensions;
using TestClient.User;

namespace TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			var userContext = new UserContext();

			IQueryable<User.User> userQuery = userContext.User( new UserInput
				{
					Name = "test",
					Description = "est"
				})
				.Select(e => new User.User
				{
					Name = e.Name,
					Username = e.Username
				})
				.Include(e => e.Posts(10))
					.Select(e => new Post.Post()
					{
						Content = e.Content,
						Title = e.Title
					})
					.ThenInclude(e => e.Comments(10))
						.Select(e => new Comment.Comment
						{
							Content = e.Content,
							Title = e.Title
						});
			
			Console.WriteLine(userQuery.ToString());

			var user = userQuery.ToItem();
			
			Console.WriteLine(user);
		}
	}
}
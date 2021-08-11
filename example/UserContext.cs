using System;
using System.Collections.Generic;
using System.Net.Http;
using LinqToGraphQL.Attributes;
using LinqToGraphQL.Context;
using LinqToGraphQL.Context.Options.Builder.Configure;
using LinqToGraphQL.Set;
using LinqToGraphQL.Set.Configuration;
using LinqToGraphQL.Translator.Behavior;
using TestClient.User;

namespace TestClient
{
	public class UserContext : GraphContext
	{
		[GraphPropertyName("user")]
		[GraphPropertyNameBehavior(TranslatorBehavior.UpperCase)]
		public GraphSet<User.User> User([GraphNonNullableProperty] UserInput input)
		{
			return Set<User.User>(new object[]
			{
				input
			}, builder =>
			{
				builder.ConfigureQuery(queryBuilder =>
				{
					queryBuilder.WithType(GraphSetTypes.Query);
				});
			}); 
		}

		protected override void Configure(GraphContextConfigureOptionsBuilder graphContextConfigureOptionsBuilder)
		{
			graphContextConfigureOptionsBuilder.ConfigureSet<User.User>(builder =>
			{
				builder.WithUrl("https://example.com/graphql");

				builder.ConfigureHttp(httpBuilder =>
				{
					httpBuilder.WithMethod(HttpMethod.Post);
				});
			});
		}
	}
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using Client.Attributes;
using Client.Context;
using Client.Context.Options.Builder.Configure;
using Client.Set;
using Client.Set.Configuration;
using Client.Translator.Behavior;

namespace TestClient
{
	public class UserContext : GraphContext
	{
		[GraphPropertyName("user")]
		[GraphPropertyNameBehavior(TranslatorBehavior.UpperCase)]
		public GraphSet<User.User> User([GraphNonNullableProperty] Guid username)
		{
			return Set<User.User>(new object[]
			{
				username
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
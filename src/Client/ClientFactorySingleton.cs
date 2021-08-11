using System;
using HttpClientFactoryLite;

namespace LinqToGraphQL.Client
{
	public class ClientFactorySingleton
	{
		public IHttpClientFactory HttpClientFactory;
		
		public static readonly ClientFactorySingleton Instance = new();
		
		static ClientFactorySingleton() {  }

		public ClientFactorySingleton()
		{
			HttpClientFactory = CreateHttpClientFactory();
		}
		
		private IHttpClientFactory CreateHttpClientFactory()
		{
			var httpClientFactory = new HttpClientFactory();
            
			httpClientFactory.Register("graph",builder =>
			{
				builder.ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.github.com"))
					.SetHandlerLifetime(TimeSpan.FromMinutes(5));
			});

			return httpClientFactory;
		}
	}
}
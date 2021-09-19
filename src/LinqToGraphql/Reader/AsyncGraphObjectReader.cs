using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LinqToGraphQL.Exceptions;
using LinqToGraphQL.Extensions;

namespace LinqToGraphQL.Reader
{
	public class AsyncGraphObjectReader<T> : IAsyncEnumerable<T>
	{
		private readonly List<T> _items;

		private readonly string _query;
		
		private readonly Task<HttpResponseMessage> _httpResponseMessage;
		
		internal AsyncGraphObjectReader(string query, Task<HttpResponseMessage> httpResponseMessage)
		{
			_query = query;
			
			_httpResponseMessage = httpResponseMessage;
		}
		
		public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			var httpResponseMessage = await _httpResponseMessage;
			
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				using var jsonDocument = await JsonDocument.ParseAsync(await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken), new JsonDocumentOptions(), cancellationToken);
				
				if (jsonDocument.RootElement.TryGetProperty("errors", out var errorElement))
				{
					throw new GraphQueryExecutionException(_query, System.Text.Json.JsonSerializer.Deserialize<List<GraphQueryError>>(errorElement.GetRawText()));
				}

				if (jsonDocument.RootElement.TryGetProperty("data", out var enumerableElement))
				{
					if (enumerableElement.TryGetProperty("result", out var enumerableResultElement))
					{
						var text = enumerableResultElement.GetRawText();

						List<T> elements = new();

						if (JsonSerializerExtensions.TryDeserialize(text, out List<T> deserializedEnumerableElements))
						{
							foreach (var deserializedItem in deserializedEnumerableElements)
							{
								yield return deserializedItem;
							}
						} else if (JsonSerializerExtensions.TryDeserialize(text, out T deserializedItemElement))
						{
							yield return deserializedItemElement;
						}
					}
				}
			} else
			{
				throw new GraphRequestExecutionException(_query, httpResponseMessage);
			}
		}
	}
}
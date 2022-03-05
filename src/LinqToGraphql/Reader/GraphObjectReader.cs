using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using LinqToGraphQL.Exceptions;
using LinqToGraphQL.Extensions;

namespace LinqToGraphQL.Reader
{
	public class GraphObjectReader<T> : IEnumerable<T>
	{
		private readonly string _query;
		
		private readonly HttpResponseMessage _httpResponseMessage;
		
		internal GraphObjectReader(string query, HttpResponseMessage httpResponseMessage)
		{
			_query = query;
			
			_httpResponseMessage = httpResponseMessage;
		}

		public IEnumerator<T> GetEnumerator()
		{
			using var jsonDocument = JsonDocument.Parse(_httpResponseMessage.Content.ReadAsStream());
			
			if (jsonDocument.RootElement.TryGetProperty("errors", out var errorElement))
			{
				throw new GraphQueryExecutionException(_query, System.Text.Json.JsonSerializer.Deserialize<List<GraphQueryError>>(errorElement.GetRawText()));
			}

			if (jsonDocument.RootElement.TryGetProperty("data", out var enumerableElement))
			{
				if (enumerableElement.TryGetProperty("result", out var enumerableResultElement))
				{
					var text = enumerableResultElement.GetRawText();
					
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
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		private bool _disposed;
		
		public void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				_httpResponseMessage.Dispose();
			}

			_disposed = true;
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
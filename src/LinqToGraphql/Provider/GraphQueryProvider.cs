using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToGraphQL.Reader;
using LinqToGraphQL.Set;
using LinqToGraphQL.Set.Configuration;
using LinqToGraphQL.Translator.Expression;
using LinqToGraphQL.Translator.Query;
using LinqToGraphQL.Types;

namespace LinqToGraphQL.Provider
{
	public class GraphQueryProvider : IQueryProvider
	{

		private readonly GraphSetConfiguration _graphSetConfiguration;
		
		private readonly HttpClient _graphHttpClient;

		public GraphQueryProvider(GraphSetConfiguration graphSetConfiguration, HttpClient graphHttpClient)
		{
			_graphSetConfiguration = graphSetConfiguration;

			_graphHttpClient = graphHttpClient;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			Type elementType = expression.Type;

			try
			{
				return (IQueryable) Activator.CreateInstance(typeof(GraphSet<>).MakeGenericType(elementType), new object[]
				{
					this, expression
				});
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException ?? e;
			}
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new GraphSet<TElement>(this, expression);
		}

		public string GetQueryText(Expression expression)
		{
			return Translate(expression);
		}

		public object? Execute(Expression expression)
		{
			var result = Request(expression);

			System.Type elementType = TypeSystemHelper.GetElementType(expression.Type);

			if (result.IsSuccessStatusCode)
			{
				var graphObjectReader = Activator.CreateInstance(typeof(GraphObjectReader<>).MakeGenericType(elementType),
					BindingFlags.Instance | BindingFlags.NonPublic, null,
					new object[]
					{
						result
					},
					null);
				
				return graphObjectReader;
			}

			throw new Exception("");
		}
		
		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult) Execute(expression);
		}

		public object? ExecuteAsync(Expression expression, CancellationToken cancellationToken)
		{
			var result =  RequestAsync(expression, cancellationToken);

			System.Type elementType = TypeSystemHelper.GetElementType(expression.Type);

			var graphObjectReader = Activator.CreateInstance(typeof(AsyncGraphObjectReader<>).MakeGenericType(elementType),
				BindingFlags.Instance | BindingFlags.NonPublic, null,
				new object[]
				{
					result
				},
				null);
				
			return graphObjectReader;
		}

		private HttpResponseMessage Request(Expression expression)
		{
			var httpRequestMessage = new HttpRequestMessage();
			
			string content = Translate(expression);

			httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
			
			ModifyRequestMessageConstraints(httpRequestMessage);
			
			return _graphHttpClient.Send(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
		}

		private Task<HttpResponseMessage> RequestAsync(Expression expression, CancellationToken cancellationToken = default)
		{
			var httpRequestMessage = new HttpRequestMessage();

			string content = Translate(expression);
			
			httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");

			ModifyRequestMessageConstraints(httpRequestMessage);
			
			return _graphHttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
		}

		private void ModifyRequestMessageConstraints(in HttpRequestMessage httpRequestMessage)
		{
			if (_graphSetConfiguration.Http is not null)
			{
				httpRequestMessage.RequestUri = new Uri(_graphSetConfiguration.Http.RequestUri);

				if (_graphSetConfiguration.Http.Headers.Any())
				{
					foreach ((var headerName, var headerValue) in _graphSetConfiguration.Http.Headers)
					{
						httpRequestMessage.Headers.Add(headerName, headerValue);
					}
				}

				if (_graphSetConfiguration.Http.Method is not null)
				{
					httpRequestMessage.Method = _graphSetConfiguration.Http.Method;
				}
			}
		}

		private string Translate(Expression expression)
		{
			var includeDetails = new GraphExpressionTranslator().Translate(expression);

			var queryTranslate = new GraphQueryTranslator().Translate(_graphSetConfiguration.Query, includeDetails);
			
			return queryTranslate;
		}
	}
}
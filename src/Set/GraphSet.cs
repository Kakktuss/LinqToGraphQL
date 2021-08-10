using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Client.Provider;

namespace Client.Set
{
	public class GraphSet<T> : IQueryable<T>, IAsyncEnumerable<T>
	{
		
		private readonly GraphQueryProvider _provider;
		private readonly Expression _expression;

		public GraphSet(GraphQueryProvider provider)
		{
			if (provider is null)
				throw new ArgumentNullException("provider");

			_provider = provider;
			
			_expression = Expression.Constant(this);
		}

		public GraphSet(GraphQueryProvider provider, Expression expression)
		{
			if (provider is null)
				throw new ArgumentNullException("provider");

			if (expression is null)
				throw new ArgumentNullException("expression");

			_provider = provider;

			_expression = expression;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return ((IEnumerable<T>) _provider.Execute(_expression)).GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable) _provider.Execute(_expression)).GetEnumerator();
		}
		
		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
		{
			return ((IAsyncEnumerable<T>) _provider.ExecuteAsync(_expression)).GetAsyncEnumerator();
		}

		public Type ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		public Expression Expression
		{
			get
			{
				return _expression;
			}
		}

		public IQueryProvider Provider
		{
			get
			{
				return _provider;
			}
		}
	}
}
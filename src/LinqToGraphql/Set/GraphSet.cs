using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using LinqToGraphQL.Provider;

namespace LinqToGraphQL.Set
{
	public class GraphSet<T> : IQueryable<T>, IAsyncEnumerable<T>, IDisposable
	{
		
		private readonly GraphQueryProvider _provider;
		private Expression? _expression;

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
			if(_disposed)
				throw new ObjectDisposedException(nameof(GraphSet<T>));
			
			return ((IEnumerable<T>) _provider.Execute(_expression)).GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			if(_disposed)
				throw new ObjectDisposedException(nameof(GraphSet<T>));
			
			return ((IEnumerable) _provider.Execute(_expression)).GetEnumerator();
		}
		
		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			if(_disposed)
				throw new ObjectDisposedException(nameof(GraphSet<T>));
			
			return ((IAsyncEnumerable<T>) _provider.ExecuteAsync(_expression, cancellationToken)).GetAsyncEnumerator(cancellationToken);
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

		public override string ToString()
		{
			return _provider.GetQueryText(Expression);
		}

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				_provider?.Dispose();
				_expression = null;
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Client.Extensions;
using Client.Provider;
using Client.Set.Select;

namespace Client.Set.Include
{
	public class IncludableQueryable<TEntity, TProperty> : IIncludableQueryable<TEntity, TProperty>
	{

		private readonly IQueryable<TEntity> _queryable;
		
		private readonly IAsyncEnumerable<TEntity> _asyncEnumerable;

		public IncludableQueryable(IQueryable<TEntity> queryable)
		{
			_queryable = queryable;

			_asyncEnumerable = _queryable as IAsyncEnumerable<TEntity>;
		}

		public IEnumerator<TEntity> GetEnumerator() => _queryable.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()  => GetEnumerator();
		
		public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken()) => _asyncEnumerable.GetAsyncEnumerator();

		public Type ElementType => _queryable.ElementType;
		public Expression Expression => _queryable.Expression;
		public IQueryProvider Provider => _queryable.Provider;
	}
}
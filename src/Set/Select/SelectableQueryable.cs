using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace LinqToGraphQL.Set.Select
{
	public class SelectableQueryable<TEntity, TProperty> : ISelectableQueryable<TEntity, TProperty>
	{

		private readonly IQueryable<TEntity> _queryable;
		
		private readonly IAsyncEnumerable<TEntity> _asyncEnumerable;

		public SelectableQueryable(IQueryable<TEntity> queryable)
		{
			_queryable = queryable;

			_asyncEnumerable = _queryable as IAsyncEnumerable<TEntity>;
		}

		public IEnumerator<TEntity> GetEnumerator() => _queryable.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default) => _asyncEnumerable.GetAsyncEnumerator(default);

		public Type ElementType => _queryable.ElementType;
		public Expression Expression => _queryable.Expression;
		public IQueryProvider Provider => _queryable.Provider;
		
		public override string ToString()
		{
			return _queryable.ToString();
		}
	}
}
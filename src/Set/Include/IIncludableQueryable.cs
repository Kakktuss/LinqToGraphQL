using System.Collections.Generic;
using System.Linq;

namespace LinqToGraphQL.Set.Include
{
	public interface IIncludableQueryable<out TEntity, out TProperty> : IQueryable<TEntity>, IAsyncEnumerable<TEntity>
	{
	}
}
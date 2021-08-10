using System.Collections.Generic;
using System.Linq;

namespace Client.Set.Include
{
	public interface IIncludableQueryable<out TEntity, out TProperty> : IQueryable<TEntity>, IAsyncEnumerable<TEntity>
	{
	}
}
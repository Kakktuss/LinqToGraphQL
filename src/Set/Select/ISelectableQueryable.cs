using System.Collections.Generic;
using System.Linq;

namespace Client.Set.Select
{
	public interface ISelectableQueryable<out TEntity, out TProperty> : IQueryable<TEntity>, IAsyncEnumerable<TEntity>
	{
		
	}
}
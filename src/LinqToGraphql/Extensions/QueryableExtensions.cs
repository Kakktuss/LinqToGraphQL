using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToGraphQL.Extensions
{
	public static class QueryableExtensions
	{
		internal static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(
			this IQueryable<TSource> source)
		{
			if (source is IAsyncEnumerable<TSource> asyncEnumerable)
			{
				return asyncEnumerable;
			}

			throw new InvalidOperationException("The current source is not an async enumerable");
		}
	}
}
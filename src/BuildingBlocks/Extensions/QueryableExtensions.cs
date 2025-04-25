using System.Linq.Expressions;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Extensions;

public static class QueryableExtensions
{
     
    public static PaginatedResult<T> ToPaginatedList<T>(this IQueryable<T> source, int pageNumber, int pageSize) where T : class
    {
        int count =  source.Count();
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        List<T> items =  source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return PaginatedResult<T>.Create(items, count, pageNumber, pageSize);
    }

    public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken) where T : class
    {
        int count = await source.CountAsync(cancellationToken);
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return PaginatedResult<T>.Create(items, count, pageNumber, pageSize);
    }

    public static IOrderedQueryable<TSource> OrderByDynamic<TSource>(
        this IQueryable<TSource> source,
        IEnumerable<(Expression<Func<TSource, object>>, bool)> sortingExpressions)
    {
        IOrderedQueryable<TSource>? orderedQuery = default;

        foreach (var (keySelector, isAscending) in sortingExpressions)
        {
            if (orderedQuery == default)
            {
                orderedQuery = isAscending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
            }
            else
            {
                orderedQuery = isAscending ? orderedQuery.ThenBy(keySelector) : orderedQuery.ThenByDescending(keySelector);
            }
        }

        return orderedQuery ?? (IOrderedQueryable<TSource>)source;
    }
}


namespace Utilities.Queryable
{
    using System.Linq;
    using Utilities.Enumerable.Models;

    public static class QueryableUtils
    {
        public static IQueryable<T> GetPage<T>(this IQueryable<T> queryable, Page page)
        {
            if (page.PageSize > 0 && page.PageNumber > 0)
                return queryable.Skip(page.Skip).Take(page.PageSize);
            return queryable;
        }
    }
}
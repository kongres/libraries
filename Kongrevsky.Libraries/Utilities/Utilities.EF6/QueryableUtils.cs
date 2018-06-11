namespace Kongrevsky.Utilities.EF6
{
    #region << Using >>

    using System;
    using System.Data.Entity;
    using System.Linq;
    using Kongrevsky.Utilities.Enumerable.Models;

    #endregion

    public static class QueryableUtils
    {
        /// <summary>
        ///     Returns Page of the IQueryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static IQueryable<T> GetPage<T>(this IQueryable<T> queryable, Page page)
        {
            if (page.PageSize > 0 && page.PageNumber > 0)
                return queryable.Skip(() => page.Skip).Take(() => page.PageSize);

            return queryable;
        }

        /// <summary>
        ///     Returns Page count of the IQueryable by specified page size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int GetPageCount<T>(this IQueryable<T> queryable, int pageSize)
        {
            if (pageSize > 0)
                return (int)Math.Ceiling((double)queryable.Count() / pageSize);

            return 1;
        }
    }
}
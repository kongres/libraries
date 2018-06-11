namespace Kongrevsky.Utilities.EF6.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Kongrevsky.Utilities.Enumerable.Models;
    using Kongrevsky.Utilities.Object;
    using Z.EntityFramework.Plus;

    public class PagingQueryable<T>
    {
        public PagingQueryable(IQueryable<T> queryable, Page page)
        {
            _page = page;
            _queryable = queryable;
        }

        private IQueryable<T> _queryable { get; }
        private Page _page { get; }

        public IQueryable<T> Queryable => _queryable;
        public Page Page => _page;

        public async Task<List<T>> GetPageAsync()
        {
            var items = await _queryable.GetPage(_page).Future().ToListAsync();
            items.ForEach(x => ObjectUtils.FixDates(x));
            return items;
        }

        public async Task<PageResult<T>> GetPageResultAsync()
        {
            var totalItemCount = _queryable.DeferredCount().FutureValue();
            var items = await _queryable.GetPage(_page).Future().ToListAsync();
            items.ForEach(x => ObjectUtils.FixDates(x));
            var pageCount = _page.PageSize > 0 ? (int)Math.Ceiling((double)totalItemCount / _page.PageSize) : items.Count > 0 ? 1 : 0;

            var result = new PageResult<T>()
                         {
                                 Items = items,
                                 PageCount = pageCount,
                                 TotalItemCount = totalItemCount
                         };

            return result;
        }

        public IQueryable<T> GetQuery()
        {
            return _queryable.GetPage(_page);
        }
    }
}
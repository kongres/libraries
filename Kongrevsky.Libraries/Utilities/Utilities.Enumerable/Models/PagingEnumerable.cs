namespace Kongrevsky.Utilities.Enumerable.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kongrevsky.Utilities.Enumerable;

    public class PagingEnumerable<T>
    {
        public PagingEnumerable(IEnumerable<T> enumerable, Page page)
        {
            _page = page;
            _enumerable = enumerable;
        }

        private IEnumerable<T> _enumerable { get; }
        private Page _page { get; }

        public IEnumerable<T> Queryable => _enumerable;
        public Page Page => _page;

        public List<T> GetPage()
        {
            return _enumerable.GetPage(_page).ToList();
        }

        public PageResult<T> GetPageResult()
        {
            var totalItemCount = _enumerable.Count();
            var items = _enumerable.GetPage(_page).ToList();
            var pageCount = _page.PageSize > 0 ? (int)Math.Ceiling((double)totalItemCount / _page.PageSize) : items.Count > 0 ? 1 : 0;

            var result = new PageResult<T>()
                         {
                                 Items = items,
                                 PageCount = pageCount,
                                 TotalItemCount = totalItemCount
                         };

            return result;
        }

        public IEnumerable<T> GetQuery()
        {
            return _enumerable.GetPage(_page);
        }
    }
}
namespace Infrastructure.Repository.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Repository.Utils;
    using Kongrevsky.Utilities.Enumerable;
    using Kongrevsky.Utilities.Enumerable.Models;
    using Z.EntityFramework.Plus;

    public class PagingQueryable<T>
    {

        public PagingQueryable(IQueryable<T> data, Page page)
        {
            _page = page;

            _totalItemCount = data.DeferredCount().FutureValue();
            _queryable = EnumerableUtils.GetPage(data, _page).Future();
        }

        public Page Page => _page;
        public int TotalItemCount => _totalItemCount;
        public int PageCount => _page.PageSize > 0 ? (int)Math.Ceiling((double)TotalItemCount / _page.PageSize) : 1;


        private Page _page { get; }
        private QueryFutureEnumerable<T> _queryable { get; }
        private QueryFutureValue<int> _totalItemCount { get; }

        public List<T> PageToList()
        {
            var pageToList = _queryable.ToList();
            pageToList.ForEach(x => ObjectUtils.FixDates(x));
            return pageToList;
        }

        public async Task<List<T>> PageToListAsync()
        {
            var pageToList = await _queryable.ToListAsync();
            pageToList.ForEach(x => ObjectUtils.FixDates(x));
            return pageToList;
        }
    }

}
namespace Kongrevsky.Utilities.Enumerable.Models
{
    #region << Using >>

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    #endregion

    public class PagingModel<T> : PagingModel
    {
        [ReadOnly(true)]
        public List<T> Items { get; private set; } = new List<T>();

        public void SetItems(IEnumerable<T> items)
        {
            Items = items.ToList();
        }

        public void SetResult(IEnumerable<T> items, int totalItems, int totalPages)
        {
            SetItems(items);
            SetTotals(totalItems, totalPages);
        }
        public void SetResult(PageResult<T> pageResult)
        {
            SetItems(pageResult.Items);
            SetTotals(pageResult.TotalItemCount, pageResult.PageCount);
        }
    }

    public class PagingModel
    {
        public string Search { get; set; }

        public bool IsDesc { get; set; }

        public string OrderProperty { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalItems { get; private set; }

        public int TotalPages { get; private set; }

        public void SetTotals(int totalItems, int totalPages)
        {
            TotalItems = totalItems;
            TotalPages = totalPages;
        }
    }
}
namespace Kongrevsky.Infrastructure.Models
{
    #region << Using >>

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    #endregion

    public class PagingModel<T> : PagingModel
    {
        #region Properties

        [ReadOnly(true)]
        public List<T> Items { get; private set; }

        public List<string> LoadProperties { get; set; }

        #endregion

        #region Constructors

        public PagingModel()
        {
            PageNumber = 1;
            PageSize = 10;
            LoadProperties = new List<string>();
            Items = new List<T>();
        }

        #endregion

        public void SetItems(IEnumerable<T> items)
        {
            Items = items.ToList();
        }

        public void SetResult(IEnumerable<T> items, int totalItems, int totalPages)
        {
            SetItems(items);
            SetTotals(totalItems, totalPages);
        }
    }

    public class PagingModel
    {
        #region Properties

        public string Search { get; set; }

        /// <summary>
        /// Item: "propertyName1==value1||propertyName2==value2" or "propertyName==" (to check for null or empty)
        /// </summary>
        public List<string> Filters { get; set; }

        /// <summary>
        /// Distinct result by this field
        /// </summary>
        public string Distinct { get; set; }

        public bool IsDesc { get; set; }

        public string OrderProperty { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; private set; }

        public int TotalPages { get; private set; }

        #endregion

        #region Constructors

        public PagingModel()
        {
            PageNumber = 1;
            PageSize = 10;
            Filters = new List<string>();
        }

        #endregion

        public void SetTotals(int totalItems, int totalPages)
        {
            TotalItems = totalItems;
            TotalPages = totalPages;
        }
    }
}
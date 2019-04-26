namespace Kongrevsky.Infrastructure.Repository.Models
{
    #region << Using >>

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Kongrevsky.Utilities.EF6.Models;
    using Kongrevsky.Utilities.Enumerable.Models;

    #endregion

    public class RepositoryPagingModel<T> : PagingModel<T>
    {
        public RepositoryPagingModel()
        {
            LoadProperties = new List<string>();
            Filters = new List<string>();
        }

        public List<string> LoadProperties { get; set; }

        /// <summary>
        ///     Item: "propertyName1==value1||propertyName2==value2" or "propertyName==" (to check for null or empty)
        /// </summary>
        public List<string> Filters { get; set; }

        /// <summary>
        ///     Distinct result by this field
        /// </summary>
        public string Distinct { get; set; }

        public async Task SetResult(PagingQueryable<T> pagingQueryable)
        {
            if (IsTotalsOnly)
                SetTotals(await pagingQueryable.GetTotalResultAsync());
            else
                SetResult(await pagingQueryable.GetPageResultAsync());
        }
    }
}
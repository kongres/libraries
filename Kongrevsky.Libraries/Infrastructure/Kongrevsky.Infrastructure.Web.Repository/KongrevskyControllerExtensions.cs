namespace Kongrevsky.Infrastructure.Web.Repository
{
    #region << Using >>

    using System.Linq;
    using Kongrevsky.Infrastructure.Repository.Models;
    using Kongrevsky.Infrastructure.Repository.Utils;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    public static class KongrevskyControllerExtensions
    {
        public static OkObjectResult OkPaging<TController, TItem>(this TController controller, RepositoryPagingModel<TItem> filter)
                where TController : Controller
                where TItem : class
        {
            return filter.LoadProperties?.Any() ?? false ? new OkObjectResult(RepositoryPagingModelUtils.ExcludeIgnoredPropertiesPagingModel(filter, filter.LoadProperties, typeof(TItem))) : controller.Ok(filter);
        }
    }
}
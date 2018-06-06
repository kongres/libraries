namespace Kongrevsky.Infrastructure.Web.ActionFilters
{
    #region << Using >>

    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Kongrevsky.Infrastructure.Web.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    #endregion

    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                context.Result = new ObjectResult(new ErrorRequestModel(context.ModelState.Select(x =>
                                                                                                  {
                                                                                                      var modelError = x.Value.Errors.FirstOrDefault();
                                                                                                      return modelError != null ? new KeyValuePair<string, string>(x.Key, modelError.ErrorMessage) : new KeyValuePair<string, string>(x.Key, "");
                                                                                                  }).ToList()))
                                 { StatusCode = (int?)HttpStatusCode.BadRequest };
            }

            base.OnActionExecuting(context);
        }
    }
}
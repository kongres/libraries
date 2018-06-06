namespace Kongrevsky.Infrastructure.Web.ActionFilters
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using Kongrevsky.Infrastructure.Web.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    #endregion

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CheckModelForNullAttribute : ActionFilterAttribute
    {
        private readonly Func<IDictionary<string, object>, bool> _validate;

        public CheckModelForNullAttribute()
                : this(arguments => arguments.Values.Contains(null)) { }

        public CheckModelForNullAttribute(Func<IDictionary<string, object>, bool> checkCondition)
        {
            this._validate = checkCondition;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (this._validate(context.ActionArguments))
            {
                context.Result = new BadRequestObjectResult(new ErrorRequestModel("Parameters are not valid."));
            }

            base.OnActionExecuting(context);
        }
    }
}
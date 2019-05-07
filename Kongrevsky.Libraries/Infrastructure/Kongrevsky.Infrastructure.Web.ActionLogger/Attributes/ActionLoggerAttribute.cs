namespace Kongrevsky.Infrastructure.Web.ActionLogger.Attributes
{
    #region << Using >>

    using System.Collections.Generic;
    using System.Linq;
    using Kongrevsky.Utilities.Web;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public class ActionLoggerAttribute : ActionFilterAttribute
    {
        public const string ActionArgumentsKey = "ActionArguments";
        public const string ExcludeFromLogs = "ExcludeFromLogs";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items[ActionArgumentsKey] = context.ActionArguments ?? new Dictionary<string, object>();
            context.HttpContext.Items[ExcludeFromLogs] = context.ActionDescriptor.GetAttributes<ActionLogAttribute>().FirstOrDefault()?.NoLog;
            base.OnActionExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
            var actionLogger = context.HttpContext.RequestServices.GetService<IActionLogger>();
            actionLogger.LogAction(context);
        }
    }
}
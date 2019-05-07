namespace Kongrevsky.Infrastructure.Web.ActionLogger
{
    #region << Using >>

    using System;
    using Kongrevsky.Infrastructure.Web.ActionLogger.Models;
    using Microsoft.AspNetCore.Mvc.Filters;

    #endregion

    public interface IActionLogger
    {
        void SetOptions(ActionLoggerOptions options);

        void SetOptions(Action<ActionLoggerOptions> action);

        void LogAction(FilterContext context);
    }
}
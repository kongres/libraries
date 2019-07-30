namespace Kongrevsky.Infrastructure.Web.ActionLogger.Attributes
{
    #region << Using >>

    using System;
    using Kongrevsky.Infrastructure.Web.ActionLogger.Models;
    using Microsoft.AspNetCore.Mvc.Filters;

    #endregion

    public class ActionLogAttribute : ActionFilterAttribute
    {
        public ActionLogAttribute()
        {
            ActionSuppressorType = ActionSuppressorType.No;
            NoLog = false;
        }

        /// <summary>
        /// Custom name of Activity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Not log these types of Activity (Default: <see cref="Kongrevsky.Infrastructure.Web.ActionLogger.Models.ActionSuppressorType.No"/>)
        /// </summary>
        public ActionSuppressorType ActionSuppressorType { get; set; }

        /// <summary>
        /// If True then exclude from logging (Default: false)
        /// </summary>
        public bool NoLog { get; set; }
    }
}
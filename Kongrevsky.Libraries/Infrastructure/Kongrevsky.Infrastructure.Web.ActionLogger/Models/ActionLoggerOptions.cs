namespace Kongrevsky.Infrastructure.Web.ActionLogger.Models
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Kongrevsky.Utilities.Object;
    using Microsoft.AspNetCore.Http;

    #endregion

    public class ActionLoggerOptions
    {
        /// <summary>
        ///     What Action Types should be logged for each HTTP method (Default: All types for all methods)
        /// </summary>
        public Dictionary<string, List<ActionLogType>> ActionTypesForHttpMethods { get; set; } = new Dictionary<string, List<ActionLogType>>(StringComparer.OrdinalIgnoreCase)
                                                                                                 {
                                                                                                         { HttpMethods.Get, new List<ActionLogType>() { ActionLogType.BadRequest, ActionLogType.Error, ActionLogType.Warning } }
                                                                                                 };

        /// <summary>
        /// Format object by Type
        /// </summary>
        public Dictionary<Type, Func<object, string>> DataFormatters { get; set; } = new Dictionary<Type, Func<object, string>>();


        public Func<object, string> EntityFormatter { get; set; } = o => o?.ToString();

        /// <summary>
        /// Action when an Exception in logger
        /// </summary>
        public Action<ActionLog> LogAction { get; set; } = actionLog =>
                                                           {
                                                               var log = new StringBuilder();
                                                               log.AppendLine($"{DateTime.UtcNow:u} {actionLog.LogType:G} - {actionLog.Name}");
                                                               log.AppendLine($"Input Data: {actionLog.InputData}");
                                                               log.AppendLine($"Output Data: {actionLog.OutputData}");
                                                               Console.WriteLine(log);
                                                           };

        /// <summary>
        /// Action when an Exception in logger
        /// </summary>
        public Action<Exception> LogException { get; set; } = exception => Console.WriteLine($"Exception in {nameof(ActionLogger)}: {exception.GetExceptionDetails()}");

        /// <summary>
        /// Max Depth for parsing object
        /// </summary>
        public int MaxDepth { get; set; } = -1;
    }
}
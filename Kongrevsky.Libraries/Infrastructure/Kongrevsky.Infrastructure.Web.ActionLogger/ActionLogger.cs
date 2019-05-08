namespace Kongrevsky.Infrastructure.Web.ActionLogger
{
    #region << Using >>

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using Kongrevsky.Infrastructure.Web.ActionLogger.Attributes;
    using Kongrevsky.Infrastructure.Web.ActionLogger.Models;
    using Kongrevsky.Utilities.Object;
    using Kongrevsky.Utilities.String;
    using Kongrevsky.Utilities.Web;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;

    #endregion

    public class ActionLogger : IActionLogger
    {
        public ActionLogger()
        {
            _options = new ActionLoggerOptions();
        }

        private ActionLoggerOptions _options { get; set; }

        public void SetOptions(ActionLoggerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void SetOptions(Action<ActionLoggerOptions> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            action.Invoke(_options);
        }

        public void LogAction(ResultExecutedContext context)
        {
            try
            {
                var responseStatusCode = context.HttpContext.Response.GetHttpStatusCode();

                ActionLogType logType;
                switch (responseStatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        logType = ActionLogType.Info;
                        break;
                    case HttpStatusCode.BadRequest:
                        logType = ActionLogType.BadRequest;
                        break;
                    case HttpStatusCode.InternalServerError:
                        logType = ActionLogType.Error;
                        break;
                    default:
                        logType = ActionLogType.Warning;
                        break;
                }

                if (_options.ActionTypesForHttpMethods.TryGetValue(context.HttpContext.Request.Method, out var allowedActionLogTypes) && !allowedActionLogTypes.Contains(logType))
                    return;

                var actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

                var logActionAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<ActionLogAttribute>();

                if (logActionAttribute != null)
                {
                    if (logActionAttribute.NoLog)
                        return;

                    var suppressorType = logActionAttribute.ActionSuppressorType;
                    if (suppressorType.HasFlag(ActionSuppressorType.All) ||
                        suppressorType.HasFlag(ActionSuppressorType.Info) && logType == ActionLogType.Info ||
                        suppressorType.HasFlag(ActionSuppressorType.Error) && logType == ActionLogType.Error ||
                        suppressorType.HasFlag(ActionSuppressorType.BadRequest) && logType == ActionLogType.BadRequest ||
                        suppressorType.HasFlag(ActionSuppressorType.Warning) && logType == ActionLogType.Warning)
                        return;
                }

                var action = logActionAttribute?.Name ?? actionDescriptor.ActionName.SplitCamelCase();

                var inputData = FormatInputData(context.HttpContext.Items[ActionLoggerAttribute.ActionArgumentsKey] as IDictionary<string, object> ?? new Dictionary<string, object>());

                var outputData = context.Result is ObjectResult objectResult && objectResult.Value != null ?  GetFormattedObject(objectResult.Value) : string.Empty;

                _options?.LogAction?.Invoke(new ActionLog()
                                            {
                                                    LogType = logType,
                                                    Name = action,
                                                    InputData = inputData.ToString(),
                                                    OutputData = outputData,
                                                    ResponseStatusCode = (int)responseStatusCode
                                            });
            }
            catch (Exception e)
            {
                _options?.LogException?.Invoke(e);
            }
        }

        private StringBuilder FormatInputData(IDictionary<string, object> inputDataDictionary)
        {
            var inputData = new StringBuilder();

            foreach (var inputObj in inputDataDictionary)
            {
                inputData.AppendLine(GetFormattedObject(inputObj.Value));
                inputData.AppendLine();
            }

            return inputData;
        }

        private string GetFormattedObject(object obj, int indentTabCount = 0)
        {
            var tabs = new string('\t', indentTabCount);
            var str = new StringBuilder();

            if (_options.MaxDepth > -1 && indentTabCount > _options.MaxDepth)
            {
                str.AppendLine(tabs + "null");
                return str.ToString();
            }

            if (obj == null)
            {
                str.AppendLine(tabs + "null");
                return str.ToString();
            }

            var type = obj.GetType();

            if (_options.DataFormatters.TryGetValue(type, out var dataFormatter))
            {
                str.AppendLine(tabs + dataFormatter(obj));
                return str.ToString();
            }

            if (type.IsSimple())
            {
                str.AppendLine(tabs + $"{obj}");
                return str.ToString();
            }

            var properties = type.GetProperties().ToList();
            foreach (var property in properties)
            {
                var logPropertyTypeAttribute = property.GetCustomAttribute<LogPropertyAttribute>();

                var name = string.IsNullOrEmpty(logPropertyTypeAttribute?.PropertyName) ? property.Name : logPropertyTypeAttribute.PropertyName;
                if (property.PropertyType.IsSimple())
                {
                    switch (logPropertyTypeAttribute?.PropertyType)
                    {
                        case LogPropertyType.Identifier:
                            if (name.EndsWith("id", StringComparison.CurrentCultureIgnoreCase) && !string.Equals(name, "id", StringComparison.CurrentCultureIgnoreCase))
                                name = name.Substring(0, name.Length - 2);
                            var id = property.GetValue(obj, null);
                            try
                            {
                                str.AppendLine(tabs + $"{name}: {_options.EntityFormatter(id)}");
                            }
                            catch (Exception e)
                            {
                                str.AppendLine(tabs + $"{name}: {id} (Type: Unknown");
                            }

                            continue;
                        case LogPropertyType.Password:
                            str.AppendLine(tabs + $"{name}: {new string('*', 6)}");
                            continue;
                        default:
                            str.AppendLine(tabs + $"{name}: {property.GetValue(obj, null) ?? "-"}");
                            continue;
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
                {
                    var enumerable = property.GetValue(obj, null) as IEnumerable<object>;
                    str.AppendLine(tabs + $"{name}:");

                    var objects = enumerable?.ToList();
                    if (objects == null || !objects.Any())
                    {
                        str.AppendLine(tabs + "\t" + "No records");
                        continue;
                    }

                    foreach (var o in objects)
                    {
                        switch (logPropertyTypeAttribute?.PropertyType)
                        {
                            case LogPropertyType.Identifier:
                                str.AppendLine(tabs + "\t" + $"{name}: {_options.EntityFormatter(o)}");
                                continue;
                            case LogPropertyType.Password:
                                str.AppendLine(tabs + "\t" + $"{new string('*', 6)}");
                                continue;
                            default:
                                str.AppendLine(tabs + "\t" + $"{o ?? "-"}");
                                continue;
                        }
                    }
                }
                else
                {
                    str.AppendLine(tabs + $"{property.Name}:");
                    str.AppendLine(GetFormattedObject(property.GetValue(obj, null), indentTabCount + 1));
                }
            }

            return str.ToString();
        }
    }
}
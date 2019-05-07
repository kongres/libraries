namespace Kongrevsky.Infrastructure.Web.ActionLogger
{
    #region << Using >>

    using System;
    using Kongrevsky.Infrastructure.Web.ActionLogger.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    #endregion

    public static class ActionLoggerExtensions
    {
        public static IServiceCollection AddActionLogger(this IServiceCollection services, Action<ActionLoggerOptions> setupAction = null)
        {
            services.Configure(setupAction ?? (Action<ActionLoggerOptions>)(opts => { }));
            services.AddSingleton<IActionLogger>(provider =>
                                                 {
                                                     var actionLogger = new ActionLogger();
                                                     var actionLoggerOptions = provider.GetService<IOptions<ActionLoggerOptions>>();
                                                     actionLogger.SetOptions(actionLoggerOptions.Value);
                                                     return actionLogger;
                                                 });
            return services;
        }
    }
}
namespace Kongrevsky.Infrastructure.Web.ActionLogger
{
    #region << Using >>

    using System;
    using Kongrevsky.Infrastructure.Web.ActionLogger.Models;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class ActionLoggerExtensions
    {
        public static IServiceCollection AddActionLogger(this IServiceCollection services, Action<ActionLoggerOptions> setupAction = null)
        {
            services.Configure(setupAction ?? (Action<ActionLoggerOptions>)(opts => { }));
            services.AddSingleton<IActionLogger, ActionLogger>();
            return services;
        }
    }
}
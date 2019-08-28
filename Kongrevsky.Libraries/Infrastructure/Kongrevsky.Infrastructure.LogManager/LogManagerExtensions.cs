namespace Kongrevsky.Infrastructure.LogManager
{
    #region << Using >>

    using System;
    using Kongrevsky.Infrastructure.LogManager.Infrastructure;
    using Kongrevsky.Infrastructure.LogManager.Models;
    using Kongrevsky.Infrastructure.Repository.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class LogManagerExtensions
    {
        public static void AddLogManager(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.AddOptions();
            services.Configure<LogManagerOptions>(configurationSection);

            var logManagerOptions = configurationSection.Get<LogManagerOptions>();
            services.AddKongrevskyRepository<LogDbContext>(logManagerOptions.ConnectionString);
            services.AddTransient<ILogManager, LogManager>();
        }

        public static void AddLogManager(this IServiceCollection services, Func<IConfigurationSection> configurationSection)
        {
            services.AddLogManager(configurationSection());
        }

        public static void AddLogManager(this IServiceCollection services, Action<LogManagerOptions> configurationSection)
        {
            var options = new LogManagerOptions();
            configurationSection(options);

            services.AddOptions();
            services.Configure<LogManagerOptions>(configurationSection);

            services.AddKongrevskyRepository<LogDbContext>(options.ConnectionString);
            services.AddTransient<ILogManager, LogManager>();
        }
    }
}
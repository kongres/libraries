namespace Kongrevsky.Infrastructure.ActiveDirectoryManager
{
    #region << Using >>

    using System;
    using Kongrevsky.Infrastructure.ActiveDirectoryManager.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class ActiveDirectoryExtensions
    {
        public static void AddKongrevskyActiveDirectoryManager(this IServiceCollection services, IConfigurationSection configurationSection, bool isFake)
        {
            services.AddOptions();
            services.Configure<ActiveDirectoryOptions>(configurationSection);

            if (isFake)
                services.AddTransient<IActiveDirectoryManager, FakeActiveDirectoryManager>();
            else
                services.AddTransient<IActiveDirectoryManager, ActiveDirectoryManager>();
        }

        public static void AddKongrevskyActiveDirectoryManager(this IServiceCollection services, Func<IConfigurationSection> configurationSection, bool isFake)
        {
            services.AddKongrevskyActiveDirectoryManager(configurationSection(), isFake);
        }
    }
}
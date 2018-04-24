namespace Kongrevsky.Infrastructure.FileManager
{
    #region << Using >>

    using System;
    using Kongrevsky.Infrastructure.FileManager.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class FileManagerExtensions
    {
        public static void AddKongrevskyFileManager(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.AddOptions();
            services.Configure<FileManagerOptions>(configurationSection);
            services.AddSingleton<IFileManager, FileManager>();
        }

        public static void AddKongrevskyFileManager(this IServiceCollection services, Func<IConfigurationSection> configurationSection)
        {
            services.AddKongrevskyFileManager(configurationSection());
        }
        public static void AddKongrevskyFileManager(this IServiceCollection services, Action<FileManagerOptions> configurationSection)
        {
            services.AddOptions();
            services.Configure<FileManagerOptions>(configurationSection);
            services.AddSingleton<IFileManager, FileManager>();
        }
    }
}
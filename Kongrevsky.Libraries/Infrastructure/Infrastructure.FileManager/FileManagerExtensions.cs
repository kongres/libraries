namespace Kongrevsky.Infrastructure.FileManager
{
    #region << Using >>

    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class FileManagerExtensions
    {
        public static void AddKongrevskyFileManager(this IServiceCollection services)
        {
            services.AddSingleton<IFileManager, FileManager>();
        }
    }
}
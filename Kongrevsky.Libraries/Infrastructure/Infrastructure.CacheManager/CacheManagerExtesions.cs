namespace Kongrevsky.Infrastructure.CacheManager
{
    #region << Using >>

    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class CacheManagerExtensions
    {
        public static void RegisterDefaultCacheManager(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheManager, CacheManager>();
        }
    }
}
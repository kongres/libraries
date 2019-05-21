namespace Kongrevsky.Infrastructure.DistributedCacheManager
{
    #region << Using >>

    using System;
    using System.Linq;
    using Kongrevsky.Infrastructure.DistributedCacheManager.Models;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class DistributedCacheManagerExtensions
    {
        public static void AddKongrevskyDistributedCacheManager(this IServiceCollection services, Action<DistributedCacheOptions> config = null)
        {
            if (services.All(x => x.ServiceType != typeof(IDistributedCache)))
                throw new Exception($"{nameof(IDistributedCache)} has not been registered");

            services.AddOptions();
            if (config == null)
                config = options => { };
            services.Configure(config);
            services.AddSingleton<IDistributedCacheManager, DistributedCacheManager>();
        }
    }
}
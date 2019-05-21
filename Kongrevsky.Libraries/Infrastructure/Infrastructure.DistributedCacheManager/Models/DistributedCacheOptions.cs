namespace Kongrevsky.Infrastructure.DistributedCacheManager.Models
{
    #region << Using >>

    using System;
    using Newtonsoft.Json;

    #endregion

    public class DistributedCacheOptions
    {
        public DistributedCacheOptions()
        {
            DefaultAbsoluteExpiration = TimeSpan.FromMinutes(1);
            DefaultSlidingExpiration = TimeSpan.FromMinutes(1);
        }

        public TimeSpan DefaultAbsoluteExpiration { get; set; }

        public TimeSpan DefaultSlidingExpiration { get; set; }

        public JsonSerializerSettings DefaultSerializerSettings { get; set; }
    }
}
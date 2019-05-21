namespace Kongrevsky.Infrastructure.DistributedCacheManager
{
    #region << Using >>

    using System;
    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.DistributedCacheManager.Models;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    #endregion

    public class DistributedCacheManager : IDistributedCacheManager
    {
        public DistributedCacheManager(IDistributedCache distributedCache, IOptions<DistributedCacheOptions> options)
        {
            _distributedCache = distributedCache;
            _options = options.Value;
        }

        private IDistributedCache _distributedCache { get; }

        private DistributedCacheOptions _options { get; }


        public void SetValue<T>(string key, T obj, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, JsonSerializerSettings serializerSettings = null)
        {
            _distributedCache.SetString(key, JsonConvert.SerializeObject(obj, serializerSettings ?? _options.DefaultSerializerSettings), new DistributedCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration ?? _options.DefaultAbsoluteExpiration).SetSlidingExpiration(slidingExpiration ?? _options.DefaultSlidingExpiration));
        }

        public T GetOrSetValue<T>(string key, Func<T> expr, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, JsonSerializerSettings serializerSettings = null)
        {
            var value = _distributedCache.GetString(key);

            if (value == null)
            {
                value = JsonConvert.SerializeObject(expr(), serializerSettings ?? _options.DefaultSerializerSettings); 
                _distributedCache.SetString(key, value, new DistributedCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration ?? _options.DefaultAbsoluteExpiration).SetSlidingExpiration(slidingExpiration ?? _options.DefaultSlidingExpiration));
            }

            return JsonConvert.DeserializeObject<T>(value, serializerSettings ?? _options.DefaultSerializerSettings);
        }

        public async Task<T> GetOrSetValueAsync<T>(string key, Func<Task<T>> expr, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, JsonSerializerSettings serializerSettings = null)
        {
            var value = await _distributedCache.GetStringAsync(key);

            if (value == null)
            {
                value = JsonConvert.SerializeObject(expr(), serializerSettings ?? _options.DefaultSerializerSettings);
                await _distributedCache.SetStringAsync(key, value, new DistributedCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration ?? _options.DefaultAbsoluteExpiration).SetSlidingExpiration(slidingExpiration ?? _options.DefaultSlidingExpiration));
            }

            return JsonConvert.DeserializeObject<T>(value, serializerSettings ?? _options.DefaultSerializerSettings);
        }

        public bool TryGetValue<T>(string key, out T value, JsonSerializerSettings serializerSettings = null)
        {
            var valueStr = _distributedCache.GetString(key);
            if (valueStr == null)
            {
                value = default(T);
                return false;
            }

            value = JsonConvert.DeserializeObject<T>(valueStr, serializerSettings ?? _options.DefaultSerializerSettings);
            return true;
        }

        public void Refresh(string key)
        {
            _distributedCache.Refresh(key);
        }

        public Task RefreshAsync(string key)
        {
            return _distributedCache.RefreshAsync(key);
        }

        public void Remove(string key)
        {
            _distributedCache.Remove(key);
        }

        public Task RemoveAsync(string key)
        {
            return _distributedCache.RemoveAsync(key);
        }
    }
}
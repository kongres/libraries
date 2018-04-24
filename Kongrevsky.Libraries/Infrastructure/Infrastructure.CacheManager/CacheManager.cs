namespace Kongrevsky.Infrastructure.CacheManager
{
    #region << Using >>

    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;

    #endregion

    public class CacheManager : ICacheManager
    {
        #region Properties

        private IMemoryCache _memoryCache { get; }

        #endregion

        #region Constructors

        public CacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #endregion

        #region Interface Implementations

        public void SetValue<T>(string userKey, string key, T obj, TimeSpan? expiration = null)
        {
            key = string.IsNullOrEmpty(userKey) ? key : userKey + key;

            _memoryCache.Set(key, obj, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration ?? TimeSpan.FromSeconds(30)));
        }

        public T GetOrSetValue<T>(string userKey, string key, Func<T> expr, TimeSpan? expiration = null)
        {
            key = string.IsNullOrEmpty(userKey) ? key : userKey + key;

            if (!_memoryCache.TryGetValue(key, out T obj))
                obj = _memoryCache.Set(key, expr(), new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration ?? TimeSpan.FromSeconds(30)));

            return obj;
        }

        public Task<T> GetOrSetValueAsync<T>(string userKey, string key, Func<Task<T>> expr, TimeSpan? expiration = null)
        {
            return Task.Run(() =>
                            {
                                key = string.IsNullOrEmpty(userKey) ? key : userKey + key;

                                if (!_memoryCache.TryGetValue(key, out T obj))
                                    lock (key + nameof(CacheManager))
                                    {
                                        var value = expr().Result;
                                        obj = _memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration ?? TimeSpan.FromSeconds(30)));
                                    }

                                return obj;
                            });
        }

        public bool TryGetValue<T>(string userKey, string hash, out T value)
        {
            hash = string.IsNullOrEmpty(userKey) ? hash : userKey + hash;

            return _memoryCache.TryGetValue(hash, out value);
        }

        #endregion
    }
}
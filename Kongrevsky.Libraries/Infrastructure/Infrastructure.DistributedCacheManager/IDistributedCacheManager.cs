namespace Kongrevsky.Infrastructure.DistributedCacheManager
{
    #region << Using >>

    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    #endregion

    public interface IDistributedCacheManager
    {
        /// <summary>
        ///     Sets value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiration"></param>
        /// <param name="slidingExpiration"></param>
        /// <param name="serializerSettings"></param>
        void SetValue<T>(string key, T obj, TimeSpan? expiration = null, TimeSpan? slidingExpiration = null, JsonSerializerSettings serializerSettings = null);

        /// <summary>
        ///     Returns existing value or adds it and returns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expr"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="slidingExpiration"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        T GetOrSetValue<T>(string key, Func<T> expr, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, JsonSerializerSettings serializerSettings = null);

        /// <summary>
        ///     Async returns existing value or adds it and returns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expr"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="slidingExpiration"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        Task<T> GetOrSetValueAsync<T>(string key, Func<Task<T>> expr, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, JsonSerializerSettings serializerSettings = null);

        /// <summary>
        ///     Tries to get value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        bool TryGetValue<T>(string key, out T value, JsonSerializerSettings serializerSettings = null);

        /// <summary>
        ///     Refreshes an item in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key"></param>
        void Refresh(string key);

        /// <summary>
        ///     Refreshes an item in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RefreshAsync(string key);


        /// <summary>
        ///     Removes a cache item based on its string key.
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        ///     Removes a cache item based on its string key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveAsync(string key);
    }
}
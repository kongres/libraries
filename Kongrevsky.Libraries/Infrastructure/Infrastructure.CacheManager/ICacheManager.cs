namespace Kongrevsky.Infrastructure.CacheManager
{
    #region << Using >>

    using System;
    using System.Threading.Tasks;

    #endregion

    public interface ICacheManager
    {
        /// <summary>
        /// Sets value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userKey"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiration"></param>
        void SetValue<T>(string userKey, string key, T obj, TimeSpan? expiration = null);

        /// <summary>
        /// Returns existing value or adds it and returns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userKey"></param>
        /// <param name="key"></param>
        /// <param name="expr"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        T GetOrSetValue<T>(string userKey, string key, Func<T> expr, TimeSpan? expiration = null);

        /// <summary>
        /// Async returns existing value or adds it and returns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userKey"></param>
        /// <param name="key"></param>
        /// <param name="expr"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        Task<T> GetOrSetValueAsync<T>(string userKey, string key, Func<Task<T>> expr, TimeSpan? expiration = null);

        /// <summary>
        /// Tries to get value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userKey"></param>
        /// <param name="hash"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue<T>(string userKey, string hash, out T value);
    }
}
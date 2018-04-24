namespace Kongrevsky.Infrastructure.CacheManager
{
    #region << Using >>

    using System;
    using System.Threading.Tasks;

    #endregion

    public interface ICacheManager
    {
        void SetValue<T>(string userKey, string key, T obj, TimeSpan? expiration = null);

        T GetOrSetValue<T>(string userKey, string key, Func<T> expr, TimeSpan? expiration = null);

        Task<T> GetOrSetValueAsync<T>(string userKey, string key, Func<Task<T>> expr, TimeSpan? expiration = null);

        bool TryGetValue<T>(string userKey, string hash, out T value);
    }
}
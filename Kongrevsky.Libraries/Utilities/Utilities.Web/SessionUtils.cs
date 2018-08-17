namespace Kongrevsky.Utilities.Web
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public static class SessionUtils
    {
        public static T Get<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            return string.IsNullOrEmpty(data) ? default(T) : JsonConvert.DeserializeObject<T>(data);
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}
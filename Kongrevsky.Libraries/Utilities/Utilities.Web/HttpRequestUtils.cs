namespace Utilities.Web
{
    #region << Using >>

    using Microsoft.AspNetCore.Http;

    #endregion

    public static class HttpRequestUtils
    {
        /// <summary>
        /// Returns root URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRootUrl(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host.Value}/";
        }
    }
}
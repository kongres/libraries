namespace Kongrevsky.Utilities.Web
{
    #region << Using >>

    using System.Net;
    using Microsoft.AspNetCore.Http;

    #endregion

    public static class HttpResponseUtils
    {
        /// <summary>
        /// Returns status code of the HttpResponse
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public static HttpStatusCode GetHttpStatusCode(this HttpResponse httpResponse)
        {
            return (HttpStatusCode)httpResponse.StatusCode;
        }
    }
}
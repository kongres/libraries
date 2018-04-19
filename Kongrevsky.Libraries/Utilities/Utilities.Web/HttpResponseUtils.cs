namespace Utilities.Web {
    using System.Net;
    using Microsoft.AspNetCore.Http;

    public static class HttpResponseUtils
    {
        public static HttpStatusCode GetHttpStatusCode(this HttpResponse httpResponse)
        {
            return (HttpStatusCode)httpResponse.StatusCode;
        }
    }
}
namespace Utilities.Web
{
    using Microsoft.AspNetCore.Http;

    public static class HttpRequestUtils
    {
        public static string GetRootUrl(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host.Value}/";
        }
    }
}

namespace Kongrevsky.Infrastructure.Web.Extensions
{
    #region << Using >>

    using Kongrevsky.Infrastructure.Web.Middlewares;
    using Microsoft.AspNetCore.Builder;

    #endregion

    public static class HttpsRedirectMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpsRedirect(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpsRedirectMiddleware>();
        }
    }
}
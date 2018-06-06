namespace Kongrevsky.Infrastructure.Web.Extensions
{
    #region << Using >>

    using Kongrevsky.Infrastructure.Web.Middlewares;
    using Microsoft.AspNetCore.Builder;

    #endregion

    public static class RequestBodyStreamReusingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestBodyStreamReusing(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestBodyStreamReusingMiddleware>();
        }
    }
}
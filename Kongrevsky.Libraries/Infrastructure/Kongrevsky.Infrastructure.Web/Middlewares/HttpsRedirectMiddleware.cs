namespace Kongrevsky.Infrastructure.Web.Middlewares
{
    #region << Using >>

    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    #endregion

    public class HttpsRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpsRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var protoHeader = context.Request.Headers["X-Forwarded-Proto"].ToString();
            if (context.Request.IsHttps || protoHeader.ToLower().Equals("https"))
            {
                await _next.Invoke(context);
            }
            else
            {
                context.Response.Redirect($"https://{context.Request.Host}{context.Request.Path}");
            }
        }
    }
}
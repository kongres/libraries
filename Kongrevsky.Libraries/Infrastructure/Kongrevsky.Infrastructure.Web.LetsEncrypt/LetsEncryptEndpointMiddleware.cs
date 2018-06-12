namespace Kongrevsky.Infrastructure.Web.LetsEncrypt
{
    #region << Using >>

    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    #endregion

    public class LetsEncryptEndpointMiddleware
    {
        public LetsEncryptEndpointMiddleware(string path, RequestDelegate next, IHostingEnvironment env)
        {
            _env = env;
            _next = next;
            _path = path;
        }

        private IHostingEnvironment _env { get; }

        private RequestDelegate _next { get; }

        private string _path { get; }

        public async Task Invoke(HttpContext context)
        {
            var id = context.Request.Path.Value?.Trim('/');
            var path = _env.ContentRootPath;
            var fullPath = Path.Combine(path, _path.TrimStart('/'));
            if (Directory.Exists(fullPath))
            {
                var dirInfo = new DirectoryInfo(fullPath);
                var files = dirInfo.GetFiles();
                foreach (var fileInfo in files)
                {
                    if (fileInfo.Name == id)
                    {
                        string content;
                        var file = File.OpenText(fileInfo.FullName);
                        using (file)
                        {
                            content = file.ReadToEnd();
                        }

                        context.Response.ContentType = "application/json";

                        string jsonString = JsonConvert.SerializeObject(content);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                        return;
                    }
                }
            }

            await this._next.Invoke(context);
        }
    }

    public static class LetsEncryptEndpointMiddlewareExtensions
    {
        public static IApplicationBuilder UseLetsEncryptEndpoint(this IApplicationBuilder builder, string path = "/.well-known/acme-challenge")
        {
            return builder.Map(new PathString(path),
                               applicationBuilder => { applicationBuilder.UseMiddleware<LetsEncryptEndpointMiddleware>(path); });
        }
    }
}
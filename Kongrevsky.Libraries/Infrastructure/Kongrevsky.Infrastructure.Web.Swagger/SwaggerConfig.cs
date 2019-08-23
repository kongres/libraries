namespace Kongrevsky.Infrastructure.Web.Swagger
{
    using System.IO;
    using System.Reflection;

    public static class SwaggerConfigs
    {
        public static Stream GetIndexUI()
        {
            var manifestResourceStream = typeof(SwaggerConfigs).GetTypeInfo().Assembly.GetManifestResourceStream("Kongrevsky.Infrastructure.Web.Swagger.Resources.index.html");
            return manifestResourceStream;
        }
    }
}
namespace Kongrevsky.Infrastructure.Web.Swagger
{
    #region << Using >>

    using System;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;

    #endregion

    public class SwaggerOptions
    {
        public string SignInUrl { get; set; }
        public string XmlCommentsFile { get; set; }
        public string Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UsernameField { get; set; }
        public Func<ApiDescription, bool> AuthDocFunc { get; set; } = authDocFunc => true;
    }
}
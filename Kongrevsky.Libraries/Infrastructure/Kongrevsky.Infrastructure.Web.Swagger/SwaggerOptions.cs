namespace Kongrevsky.Infrastructure.Web.Swagger
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;

    #endregion

    public class SwaggerOptions
    {
        public class SwaggerNonOAuthOptions
        {
            public string SignInUrl { get; set; }
            public string UsernameField { get; set; }
        }

        public class SwaggerOAuthOptions
        {
            public string ClientId { get; set; }
            public string AuthorizationUrl { get; set; }
            public string TokenUrl { get; set; }
            public Dictionary<string, string> Scopes { get; set; } = new Dictionary<string, string>();
        }

        public string XmlCommentsFile { get; set; }
        public string Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool OAuthEnabled { get; set; }
        public SwaggerOAuthOptions OAuthOptions { get; set; } = new SwaggerOAuthOptions();
        public SwaggerNonOAuthOptions NonOAuthOptions { get; set; } = new SwaggerNonOAuthOptions();
        public Func<ApiDescription, bool> AuthDocFunc { get; set; } = authDocFunc => true;
    }
}
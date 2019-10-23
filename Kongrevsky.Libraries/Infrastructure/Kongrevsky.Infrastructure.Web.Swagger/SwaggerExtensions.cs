namespace Kongrevsky.Infrastructure.Web.Swagger
{
    #region << Using >>

    using System;
    using System.IO;
    using System.Reflection;
    using MicroElements.Swashbuckle.FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerUI;

    #endregion

    public static class SwaggerExtensions
    {
        public static void UseSwagger(this IApplicationBuilder app)
        {
            app.UseScopedSwagger(c => { c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value); });
            var options = app.ApplicationServices.GetService<IOptions<SwaggerOptions>>().Value;
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{options.Version}/swagger.json", $"Versioned API {options.Version}");
                c.IndexStream = SwaggerConfigs.GetIndexUI;
                c.ConfigObject.AdditionalItems["OAuthEnabled"] = options.OAuthEnabled;

                if (!options.OAuthEnabled)
                {
                    c.ConfigObject.AdditionalItems["SignInUrl"] = !string.IsNullOrEmpty(options.NonOAuthOptions.SignInUrl)
                            ? options.NonOAuthOptions.SignInUrl
                            : "/api/Auth/SignIn";
                    c.ConfigObject.AdditionalItems["UsernameField"] = !string.IsNullOrEmpty(options.NonOAuthOptions.UsernameField)
                            ? options.NonOAuthOptions.UsernameField
                            : "login";
                }
                else
                {
                    c.DocExpansion(DocExpansion.None);
                    c.OAuthClientId(options.OAuthOptions.ClientId);
                }
            });
        }

        public static void AddSwagger(this IServiceCollection services, Action<SwaggerOptions> configureOptions)
        {
            services.AddOptions();
            services.Configure(configureOptions);
            var options = new SwaggerOptions();
            configureOptions.Invoke(options);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options.Version,
                    new Info
                    {
                        Title = options.Title,
                        Version = options.Version,
                        Description = options.Description
                    });
                if (options.OAuthEnabled)
                {
                    var oAuthOptions = options.OAuthOptions;
                    c.AddSecurityDefinition("SSO", new OAuth2Scheme()
                                                   {
                                                           Type = "oauth2",
                                                           AuthorizationUrl = oAuthOptions.AuthorizationUrl,
                                                           TokenUrl = oAuthOptions.TokenUrl,
                                                           Scopes = oAuthOptions.Scopes,
                                                           Flow = "implicit"
                                                   });
                }
                else
                {
                    c.AddSecurityDefinition("BasicAuth", new BasicAuthScheme { Description = "Login" });
                    c.AddSecurityDefinition("Bearer",
                                            new ApiKeyScheme
                                            {
                                                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                                                    Name = "Authorization",
                                                    In = "header",
                                                    Type = "apiKey"
                                            });
                }

                c.DescribeAllEnumsAsStrings();
                c.IncludeXmlComments(GetXmlCommentsPath(options.XmlCommentsFile));
                c.DocumentFilter<AuthorizeDocumentFilter>(options.AuthDocFunc);
                c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
                c.OperationFilter<HidePropertiesWithPrivateSetOperationFilter>();
                c.AddFluentValidationRules();
            });
        }

        private static string GetXmlCommentsPath(string xmlCommentsFile)
        {
            var location = Assembly.GetEntryAssembly().Location;
            var directory = Path.GetDirectoryName(location);
            return Path.Combine(directory, xmlCommentsFile);
        }
    }
}
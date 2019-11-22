namespace Kongrevsky.Infrastructure.Web.Swagger
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using MicroElements.Swashbuckle.FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerUI;

    #endregion

    public static class SwaggerExtensions
    {
        public static void UseSwagger(this IApplicationBuilder app)
        {
            app.UseScopedSwagger(c =>
                                 {
                                     c.PreSerializeFilters.Add((swagger, httpReq) =>
                                                               {
                                                                   swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                                                               });
                                 });
            var options = app.ApplicationServices.GetService<IOptions<SwaggerOptions>>().Value;
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{options.Version}/swagger.json", $"Versioned API {options.Version}");
                c.IndexStream = SwaggerConfigs.GetIndexUI;
                c.ConfigObject.AdditionalItems["OAuthEnabled"] = options.OAuthEnabled;
                c.DocExpansion(DocExpansion.None);

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
                    new OpenApiInfo()
                    {
                        Title = options.Title,
                        Version = options.Version,
                        Description = options.Description
                    });
                if (options.OAuthEnabled)
                {
                    var oAuthOptions = options.OAuthOptions;
                    c.AddSecurityDefinition("SSO", new OpenApiSecurityScheme()
                                                   {
                                                           Type = SecuritySchemeType.OAuth2,
                                                           Flows = new OpenApiOAuthFlows()
                                                                   {
                                                                           Implicit = new OpenApiOAuthFlow()
                                                                                      {
                                                                                              AuthorizationUrl = new Uri(oAuthOptions.AuthorizationUrl),
                                                                                              TokenUrl = new Uri(oAuthOptions.TokenUrl),
                                                                                              Scopes = oAuthOptions.Scopes,
                                                                                      }

                                                                   }
                                                     
                                                   });
                }
                else
                {
                    c.AddSecurityDefinition("BasicAuth",new OpenApiSecurityScheme()
                                                        {
                                                                Type = SecuritySchemeType.Http,
                                                                Description = "Login"
                                                        });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                                                      {
                                                              Type = SecuritySchemeType.ApiKey,
                                                              In = ParameterLocation.Header,
                                                              Name = "Authorization",
                                                              Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                                                              
                                                      });
                }

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
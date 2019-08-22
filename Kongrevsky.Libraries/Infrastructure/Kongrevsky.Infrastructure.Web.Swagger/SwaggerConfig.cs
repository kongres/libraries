namespace Kongrevsky.Infrastructure.Web.Swagger
{
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Kongrevsky.Utilities.Web;
    using MicroElements.Swashbuckle.FluentValidation;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public static class SwaggerConfig
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0",
                             new Info
                             {
                                 Title = "DepoSched API",
                                 Version = "v1.0",
                                 Description = "Documentation of DepoSched API",
                             });
                c.AddSecurityDefinition("BasicAuth", new BasicAuthScheme { Description = "Login" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",
                });

                c.DescribeAllEnumsAsStrings();
                c.IncludeXmlComments(GetXmlCommentsPath());
                c.DocumentFilter<AuthorizeDocumentFilter>();
                c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
                c.OperationFilter<HidePropertiesWithPrivateSetOperationFilter>();
                c.AddFluentValidationRules();
            });
        }

        public static void UseSwagger(this IApplicationBuilder app)
        {
            app.UseScopedSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Versioned API v1.0");
                c.IndexStream = () =>
                {
                    var manifestResourceStream = typeof(SwaggerConfig).GetTypeInfo().Assembly.GetManifestResourceStream("DepoSched.UI.Infrastructure.Swagger.index.html");
                    return manifestResourceStream;
                };
            });
        }

        private static string GetXmlCommentsPath()
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = Path.GetDirectoryName(location);
            return Path.Combine(directory, "DepoSched.UI.xml");
        }
    }

    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.ActionDescriptor.AllowAnonymousAttributeExists())
            {
                if (operation.Parameters == null)
                    operation.Parameters = new System.Collections.Generic.List<IParameter>();

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Description = "access token (JWT)",
                    Required = false,
                    Type = "string"
                });

                if (context.ApiDescription.HttpMethod == WebRequestMethods.Http.Get)
                {
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "access_token",
                        In = "query",
                        Description = "access token (JWT)",
                        Required = false,
                        Type = "string"
                    });
                }
            }
        }
    }


    public class HidePropertiesWithPrivateSetOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var parameters = context.ApiDescription.ParameterDescriptions.Where(x => x.ModelMetadata.IsReadOnly).ToList();
            foreach (var parameter in parameters)
            {
                var item = operation.Parameters.FirstOrDefault(x => x.Name == parameter.Name);
                if (item != null)
                    operation.Parameters.Remove(item);
            }
        }
    }

    public class AuthorizeDocumentFilter : IDocumentFilter
    {
        public AuthorizeDocumentFilter(IHttpSessionContext httpSessionContext)
        {
            _httpSessionContext = httpSessionContext;
        }

        private IHttpSessionContext _httpSessionContext { get; }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Security = new System.Collections.Generic.List<System.Collections.Generic.IDictionary<string, System.Collections.Generic.IEnumerable<string>>>()
                                {
                                        new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>()
                                        {
                                                { "Bearer", new string[]{ } }
                                        }
                                };
            System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, PathItem>> removeItems;

            if (_httpSessionContext.IsAuthorized())
            {
                var apiDescriptions = context.ApiDescriptionsGroups.Items.SelectMany(x => x.Items).Where(x => (x.ActionDescriptor?.GetAttributes<AllowAnonymousAttribute>().Any() ?? false) ||
                                                                                                              (x.ActionDescriptor?.GetAttributes<AllowAnonymousFilter>().Any() ?? false) ||
                                                                                                              (x.ActionDescriptor?.GetAttributes<UserAuthorizeAttribute>().Any() ?? false)).ToList();

                removeItems = swaggerDoc.Paths.Where(x => apiDescriptions.All(a => "/" + a.RelativePath != x.Key)).ToList();

                foreach (var path in removeItems)
                    swaggerDoc.Paths.Remove(path);
            }
            else
            {
                var allowAnonymousMethods = context.ApiDescriptionsGroups.Items.SelectMany(x => x.Items).Where(x => ((x.ActionDescriptor?.GetAttributes<AllowAnonymousAttribute>().Any() ?? false) ||
                                                                                                                    (x.ActionDescriptor?.GetAttributes<AllowAnonymousFilter>().Any() ?? false)) &&
                                                                                                                    !(x.ActionDescriptor?.GetAttributes<ExcludeFromDocForAnonimousAttribute>().Any() ?? false)).ToList();

                removeItems = swaggerDoc.Paths.Where(x => allowAnonymousMethods.All(a => "/" + a.RelativePath != x.Key)).ToList();

                foreach (var path in removeItems)
                    swaggerDoc.Paths.Remove(path);
            }
        }
    }

    public class ExcludeFromDocForAnonimousAttribute : ActionFilterAttribute
    {
    }
}
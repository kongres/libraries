namespace Kongrevsky.Infrastructure.Web.Swagger
{
    using System.Collections.Generic;
    using System.Net;
    using Kongrevsky.Utilities.Web;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.ActionDescriptor.AllowAnonymousAttributeExists())
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter()
                                         {
                                                 Name = "Authorization",
                                                 In = ParameterLocation.Header,
                                                 Description = "access token (JWT)",
                                                 Required = false,
                                                 Schema = new OpenApiSchema()
                                                          {
                                                                  Type = "string"
                                                          }
                                         });

                if (context.ApiDescription.HttpMethod == WebRequestMethods.Http.Get)
                    operation.Parameters.Add(new OpenApiParameter
                                             {
                                                     Name = "access_token",
                                                     In = ParameterLocation.Header,
                                                     Description = "access token (JWT)",
                                                     Required = false,
                                                     Schema = new OpenApiSchema()
                                                              {
                                                                      Type = "string"
                                                              }
                                             });
            }
        }
    }
}
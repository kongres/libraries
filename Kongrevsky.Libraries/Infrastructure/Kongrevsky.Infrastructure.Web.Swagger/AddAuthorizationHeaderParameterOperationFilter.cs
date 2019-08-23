namespace Kongrevsky.Infrastructure.Web.Swagger
{
    using System.Collections.Generic;
    using System.Net;
    using Kongrevsky.Utilities.Web;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.ActionDescriptor.AllowAnonymousAttributeExists())
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<IParameter>();

                operation.Parameters.Add(new NonBodyParameter
                                         {
                                                 Name = "Authorization",
                                                 In = "header",
                                                 Description = "access token (JWT)",
                                                 Required = false,
                                                 Type = "string"
                                         });

                if (context.ApiDescription.HttpMethod == WebRequestMethods.Http.Get)
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
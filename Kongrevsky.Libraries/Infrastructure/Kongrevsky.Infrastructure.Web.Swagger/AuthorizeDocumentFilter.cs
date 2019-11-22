namespace Kongrevsky.Infrastructure.Web.Swagger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class AuthorizeDocumentFilter : IDocumentFilter
    {
        public AuthorizeDocumentFilter()
        {
            _funcShowApiDescription = apiDescr => true;
        }

        public AuthorizeDocumentFilter(Func<ApiDescription, bool> funcShowApiDescription)
        {
            _funcShowApiDescription = funcShowApiDescription;
        }

        private Func<ApiDescription, bool> _funcShowApiDescription { get; }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            List<KeyValuePair<string, OpenApiPathItem>> removeItems;

            var apiDescriptions = context.ApiDescriptions.Where(_funcShowApiDescription).ToList();

            removeItems = swaggerDoc.Paths.Where(x => apiDescriptions.All(a => "/" + a.RelativePath != x.Key)).ToList();

            foreach (var path in removeItems)
                swaggerDoc.Paths.Remove(path.Key);
        }
    }
}
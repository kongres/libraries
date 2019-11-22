namespace Kongrevsky.Infrastructure.Web.Swagger
{
    using System.Linq;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class HidePropertiesWithPrivateSetOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
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
}
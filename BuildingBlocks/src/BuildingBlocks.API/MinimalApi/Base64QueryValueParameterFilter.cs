using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backbone.BuildingBlocks.API.MinimalApi;

public class Base64QueryValueParameterFilter : IParameterFilter
{
    public void Apply(IOpenApiParameter parameter, ParameterFilterContext context)
    {
        var type = context.ParameterInfo?.ParameterType;
        if (type != typeof(Base64QueryValue) && type != typeof(Base64QueryValue?))
            return;

        if (parameter.Schema is OpenApiSchema schema)
        {
            schema.Format = "byte";
            schema.ContentEncoding = "base64";
        }
    }
}

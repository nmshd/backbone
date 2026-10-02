using Backbone.Modules.Relationships.Abstractions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;

internal sealed class OpenApiOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.DeclaringType != typeof(Endpoint))
            return;

        // The legacy complex array is read from HttpRequest and still belongs in the public API description.
        operation.Parameters ??= [];
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "templates",
            In = ParameterLocation.Query,
            Schema = context.SchemaGenerator.GenerateSchema(typeof(ListRelationshipTemplatesQueryItem[]), context.SchemaRepository)
        });
    }
}

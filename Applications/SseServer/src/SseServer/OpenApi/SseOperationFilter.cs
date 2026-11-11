using Backbone.SseServer.Controllers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backbone.SseServer.OpenApi;

public class SseOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.DeclaringType != typeof(SseController) || context.MethodInfo.Name != nameof(SseController.Subscribe))
            return;

        operation.Responses!["200"] = new OpenApiResponse
        {
            Description = "A stream of event notifications, starting with ConnectionOpened. The data field is always an underscore; clients fetch event details separately.",
            Content = new Dictionary<string, IOpenApiMediaType>
            {
                ["text/event-stream"] = new OpenApiMediaType
                {
                    ItemSchema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string> { "event", "data" },
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["event"] = new OpenApiSchema { Type = JsonSchemaType.String, Examples = ["ConnectionOpened"] },
                            ["data"] = new OpenApiSchema { Type = JsonSchemaType.String, Const = "_" }
                        }
                    },
                    Examples = new Dictionary<string, IOpenApiExample>
                    {
                        ["connectionOpened"] = new OpenApiExample
                        {
                            Summary = "Initial connection notification",
                            SerializedValue = "event: ConnectionOpened\ndata: _\n\n"
                        }
                    }
                }
            }
        };
    }
}

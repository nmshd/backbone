using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.GetFileContent;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetFileContentQuery")]
public class Query : IRequest<Response>
{
    public required string Id { get; init; }
}

using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.DeleteFile;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteFileCommand")]
public class Command : IRequest
{
    public required string Id { get; init; }
}

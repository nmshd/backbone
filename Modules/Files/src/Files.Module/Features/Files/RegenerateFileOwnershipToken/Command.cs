using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.RegenerateFileOwnershipToken;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RegenerateFileOwnershipTokenCommand")]
public class Command : IRequest<Response>
{
    public required string FileId { get; init; }
}

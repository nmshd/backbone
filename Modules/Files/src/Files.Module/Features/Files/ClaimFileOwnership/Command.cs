using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.ClaimFileOwnership;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ClaimFileOwnershipCommand")]
public class Command : IRequest<Response>
{
    public required string FileId { get; init; }
    public required string OwnershipToken { get; init; }
}

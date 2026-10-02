using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.ValidateFileOwnershipToken;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ValidateFileOwnershipTokenQuery")]
public class Query : IRequest<Response>
{
    public required string FileId { get; init; }
    public required string OwnershipToken { get; init; }
}

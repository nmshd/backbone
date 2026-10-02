using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeAndAnonymizeRelationshipsOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DecomposeAndAnonymizeRelationshipsOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}

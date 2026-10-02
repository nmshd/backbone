using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationshipReactivation;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RevokeRelationshipReactivationCommand")]
public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
}

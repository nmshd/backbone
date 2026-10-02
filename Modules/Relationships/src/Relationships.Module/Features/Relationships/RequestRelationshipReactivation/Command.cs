using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RequestRelationshipReactivation;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RequestRelationshipReactivationCommand")]
public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
}

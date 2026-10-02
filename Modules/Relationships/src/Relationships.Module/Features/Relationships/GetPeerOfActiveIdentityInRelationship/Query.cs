using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.GetPeerOfActiveIdentityInRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetPeerOfActiveIdentityInRelationshipQuery")]
public class Query : IRequest<Response>
{
    public required string Id { get; init; }
}

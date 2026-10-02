using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CanEstablishRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CanEstablishRelationshipQuery")]
public class Query : IRequest<Response>
{
    public required string PeerAddress { get; init; }
}

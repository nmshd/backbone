using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RejectRelationshipCommand")]
public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
    public byte[]? CreationResponseContent { get; init; }
}

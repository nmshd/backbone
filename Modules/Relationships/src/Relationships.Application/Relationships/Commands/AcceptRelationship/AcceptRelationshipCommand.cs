using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationship;

public class AcceptRelationshipCommand : IRequest<AcceptRelationshipResponse>
{
    public required string RelationshipId { get; set; }
    public byte[]? AcceptanceContent { get; set; }
}

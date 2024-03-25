using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationship;

public class RejectRelationshipCommand : IRequest<RejectRelationshipResponse>
{
    public required RelationshipId RelationshipId { get; set; }
}

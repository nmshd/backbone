using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.TerminateRelationship;

public class TerminateRelationshipCommand : IRequest<RelationshipDTO>
{
    public required string RelationshipId { get; init; }
}

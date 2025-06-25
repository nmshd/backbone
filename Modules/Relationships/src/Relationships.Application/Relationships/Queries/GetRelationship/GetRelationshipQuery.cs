using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;

public class GetRelationshipQuery : IRequest<RelationshipDTO>
{
    public required string Id { get; init; }
}

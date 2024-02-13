using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;

public class GetRelationshipQuery : IRequest<RelationshipDTO>
{
    public required RelationshipId Id { get; set; }
}

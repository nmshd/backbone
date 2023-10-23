using Backbone.Relationships.Application.Relationships.DTOs;
using Backbone.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Relationships.Application.Relationships.Queries.GetRelationship;

public class GetRelationshipQuery : IRequest<RelationshipDTO>
{
    public RelationshipId Id { get; set; }
}

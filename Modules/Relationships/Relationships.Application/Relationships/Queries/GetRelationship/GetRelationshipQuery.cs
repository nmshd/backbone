using MediatR;
using Relationships.Application.Relationships.DTOs;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.Queries.GetRelationship;

public class GetRelationshipQuery : IRequest<RelationshipDTO>
{
    public RelationshipId Id { get; set; }
}

using MediatR;
using Relationships.Application.Relationships.DTOs;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.Queries.GetChange;

public class GetChangeRequest : IRequest<RelationshipChangeDTO>
{
    public RelationshipChangeId Id { get; set; }
}

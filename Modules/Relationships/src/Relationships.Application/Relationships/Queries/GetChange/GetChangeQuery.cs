using Backbone.Relationships.Application.Relationships.DTOs;
using Backbone.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Relationships.Application.Relationships.Queries.GetChange;

public class GetChangeQuery : IRequest<RelationshipChangeDTO>
{
    public RelationshipChangeId Id { get; set; }
}

using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetChange;

public class GetChangeQuery : IRequest<RelationshipChangeDTO>
{
    public RelationshipChangeId Id { get; set; }
}

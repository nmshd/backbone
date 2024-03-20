using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetChange;

public class GetChangeQuery : IRequest<RelationshipChangeDTO>
{
    public required RelationshipChangeId Id { get; set; }
}

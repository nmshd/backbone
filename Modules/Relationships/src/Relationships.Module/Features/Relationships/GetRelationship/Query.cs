using Backbone.Modules.Relationships.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.GetRelationship;

public class Query : IRequest<RelationshipDTO>
{
    public required string Id { get; init; }
}

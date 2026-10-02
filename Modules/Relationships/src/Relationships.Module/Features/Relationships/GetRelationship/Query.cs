using Backbone.Modules.Relationships.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.GetRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetRelationshipQuery")]
public class Query : IRequest<RelationshipDTO>
{
    public required string Id { get; init; }
}

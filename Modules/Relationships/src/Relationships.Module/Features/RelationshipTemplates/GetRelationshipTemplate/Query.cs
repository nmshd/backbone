using Backbone.Modules.Relationships.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.GetRelationshipTemplate;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetRelationshipTemplateQuery")]
public class Query : IRequest<RelationshipTemplateDTO>
{
    public required string Id { get; init; }
    public byte[]? Password { get; init; }
}

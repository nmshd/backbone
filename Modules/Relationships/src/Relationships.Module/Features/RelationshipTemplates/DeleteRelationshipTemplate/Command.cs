using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplate;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteRelationshipTemplateCommand")]
public class Command : IRequest
{
    public required string Id { get; init; }
}

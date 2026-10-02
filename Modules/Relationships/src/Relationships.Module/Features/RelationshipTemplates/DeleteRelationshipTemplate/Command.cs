using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplate;

public class DeleteRelationshipTemplateCommand : IRequest
{
    public required string Id { get; init; }
}

using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplate;

public class DeleteRelationshipTemplateCommand : IRequest
{
    public required string Id { get; init; }
}

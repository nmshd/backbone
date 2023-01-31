using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplate;

public class DeleteRelationshipTemplateCommand : IRequest<Unit>
{
    public RelationshipTemplateId Id { get; set; }
}

using MediatR;
using Relationships.Domain.Ids;

namespace Relationships.Application.RelationshipTemplates.Command.DeleteRelationshipTemplate;

public class DeleteRelationshipTemplateCommand : IRequest<Unit>
{
    public RelationshipTemplateId Id { get; set; }
}

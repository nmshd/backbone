using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;

public class CreateRelationshipCommand : IRequest<CreateRelationshipResponse>
{
    public RelationshipTemplateId RelationshipTemplateId { get; set; }
    public byte[] Content { get; set; }
}

using MediatR;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.Commands.CreateRelationship;

public class CreateRelationshipCommand : IRequest<CreateRelationshipResponse>
{
    public RelationshipTemplateId RelationshipTemplateId { get; set; }
    public byte[] Content { get; set; }
}

using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Relationships.Domain.Entities;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.Commands.CreateRelationshipTemplate;

public class CreateRelationshipTemplateResponse : IMapTo<RelationshipTemplate>
{
    public RelationshipTemplateId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}

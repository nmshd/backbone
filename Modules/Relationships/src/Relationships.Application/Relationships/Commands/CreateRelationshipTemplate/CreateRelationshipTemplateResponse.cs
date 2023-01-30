using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationshipTemplate;

public class CreateRelationshipTemplateResponse : IMapTo<RelationshipTemplate>
{
    public RelationshipTemplateId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}

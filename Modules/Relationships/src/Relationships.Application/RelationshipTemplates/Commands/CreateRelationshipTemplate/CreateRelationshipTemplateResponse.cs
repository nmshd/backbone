using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Relationships.Domain.Entities;
using Backbone.Relationships.Domain.Ids;

namespace Backbone.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class CreateRelationshipTemplateResponse : IMapTo<RelationshipTemplate>
{
    public RelationshipTemplateId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}

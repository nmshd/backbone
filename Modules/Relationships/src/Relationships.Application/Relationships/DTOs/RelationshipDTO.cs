using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Relationships.Domain.Entities;
using Backbone.Relationships.Domain.Ids;

namespace Backbone.Relationships.Application.Relationships.DTOs;

public class RelationshipDTO : IMapTo<Relationship>
{
    public RelationshipId Id { get; set; }
    public RelationshipTemplateId RelationshipTemplateId { get; set; }

    public IdentityAddress From { get; set; }
    public IdentityAddress To { get; set; }
    public IEnumerable<RelationshipChangeDTO> Changes { get; set; }

    public DateTime CreatedAt { get; set; }

    public RelationshipStatus Status { get; set; }
}

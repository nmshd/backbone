using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipDTO : IMapTo<Relationship>
{
    public required RelationshipId Id { get; set; }
    public required RelationshipTemplateId RelationshipTemplateId { get; set; }

    public required IdentityAddress From { get; set; }
    public required IdentityAddress To { get; set; }
    public required IEnumerable<RelationshipChangeDTO> Changes { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required RelationshipStatus Status { get; set; }
}

using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

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

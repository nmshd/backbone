using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipTemplateDTO : IMapTo<RelationshipTemplate>
{
    public required RelationshipTemplateId Id { get; set; }

    public required IdentityAddress CreatedBy { get; set; }
    public required DeviceId CreatedByDevice { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public byte[]? Content { get; set; }

    public required DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

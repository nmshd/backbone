using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Relationships.Domain.Entities;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.DTOs;

public class RelationshipTemplateDTO : IMapTo<RelationshipTemplate>
{
    public RelationshipTemplateId Id { get; set; }

    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public byte[] Content { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

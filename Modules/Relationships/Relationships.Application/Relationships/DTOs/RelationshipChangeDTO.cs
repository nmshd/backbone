using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Relationships.Domain.Entities;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.DTOs;

public class RelationshipChangeDTO : IMapTo<RelationshipChange>
{
    public RelationshipChangeId Id { get; set; }

    public RelationshipId RelationshipId { get; set; }

    public RelationshipChangeRequestDTO Request { get; set; }
    public RelationshipChangeResponseDTO Response { get; set; }

    public RelationshipChangeType Type { get; set; }

    public RelationshipChangeStatus Status { get; set; }
}

public class RelationshipChangeRequestDTO : IMapTo<RelationshipChangeRequest>
{
    public DateTime CreatedAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
    public byte[] Content { get; set; }
}

public class RelationshipChangeResponseDTO : IMapTo<RelationshipChangeResponse>
{
    public DateTime CreatedAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
    public byte[] Content { get; set; }
}

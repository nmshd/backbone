using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipChangeDTO : IMapTo<RelationshipChange>
{
    public required RelationshipChangeId Id { get; set; }

    public required RelationshipId RelationshipId { get; set; }

    public required RelationshipChangeRequestDTO Request { get; set; }
    public required RelationshipChangeResponseDTO Response { get; set; }

    public required RelationshipChangeType Type { get; set; }

    public required RelationshipChangeStatus Status { get; set; }
}

public class RelationshipChangeRequestDTO : IMapTo<RelationshipChangeRequest>
{
    public required DateTime CreatedAt { get; set; }
    public required IdentityAddress CreatedBy { get; set; }
    public required DeviceId CreatedByDevice { get; set; }
    public required byte[] Content { get; set; }
}

public class RelationshipChangeResponseDTO : IMapTo<RelationshipChangeResponse>
{
    public required DateTime CreatedAt { get; set; }
    public required IdentityAddress CreatedBy { get; set; }
    public required DeviceId CreatedByDevice { get; set; }
    public required byte[] Content { get; set; }
}

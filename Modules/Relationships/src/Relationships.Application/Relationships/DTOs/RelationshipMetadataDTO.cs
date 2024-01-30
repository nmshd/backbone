using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipMetadataDTO : IMapTo<Relationship>
{
    public required RelationshipId Id { get; set; }
    public required RelationshipTemplateId RelationshipTemplateId { get; set; }

    public required IdentityAddress From { get; set; }
    public required IdentityAddress To { get; set; }
    public required IEnumerable<RelationshipChangeMetadataDTO> Changes { get; set; }

    public required DateTime CreatedAt { get; set; }

    public RelationshipStatus Status { get; private set; }
}

public class RelationshipChangeMetadataDTO : IMapTo<RelationshipChange>
{
    public required RelationshipChangeId Id { get; set; }

    public required RelationshipId RelationshipId { get; set; }

    public required RelationshipChangeRequestMetadataDTO Request { get; set; }
    public required RelationshipChangeResponseMetadataDTO Response { get; set; }

    public RelationshipChangeType Type { get; set; }

    public RelationshipChangeStatus Status { get; set; }
}

public class RelationshipChangeRequestMetadataDTO : IMapTo<RelationshipChangeRequest>
{
    public DateTime CreatedAt { get; set; }
    public required IdentityAddress CreatedBy { get; set; }
    public required DeviceId CreatedByDevice { get; set; }
}

public class RelationshipChangeResponseMetadataDTO : IMapTo<RelationshipChangeResponse>
{
    public DateTime CreatedAt { get; set; }
    public required IdentityAddress CreatedBy { get; set; }
    public required DeviceId CreatedByDevice { get; set; }
}

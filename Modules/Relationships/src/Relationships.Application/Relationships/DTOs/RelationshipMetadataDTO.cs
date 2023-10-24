using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipMetadataDTO : IMapTo<Relationship>
{
    public RelationshipId Id { get; set; }
    public RelationshipTemplateId RelationshipTemplateId { get; set; }

    public IdentityAddress From { get; set; }
    public IdentityAddress To { get; set; }
    public IEnumerable<RelationshipChangeMetadataDTO> Changes { get; set; }

    public DateTime CreatedAt { get; set; }

    public RelationshipStatus Status { get; private set; }
}

public class RelationshipChangeMetadataDTO : IMapTo<RelationshipChange>
{
    public RelationshipChangeId Id { get; set; }

    public RelationshipId RelationshipId { get; set; }

    public RelationshipChangeRequestMetadataDTO Request { get; set; }
    public RelationshipChangeResponseMetadataDTO Response { get; set; }

    public RelationshipChangeType Type { get; set; }

    public RelationshipChangeStatus Status { get; set; }
}

public class RelationshipChangeRequestMetadataDTO : IMapTo<RelationshipChangeRequest>
{
    public DateTime CreatedAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
}

public class RelationshipChangeResponseMetadataDTO : IMapTo<RelationshipChangeResponse>
{
    public DateTime CreatedAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
}

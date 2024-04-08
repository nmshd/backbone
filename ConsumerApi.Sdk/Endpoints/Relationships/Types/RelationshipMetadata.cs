namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;

public class RelationshipMetadata
{
    public required string Id { get; set; }
    public required string RelationshipTemplateId { get; set; }

    public required string From { get; set; }
    public required string To { get; set; }
    public required List<RelationshipChangeMetadata> Changes { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required string Status { get; set; }
}

public class RelationshipChangeMetadata
{
    public required string Id { get; set; }

    public required string RelationshipId { get; set; }

    public required RelationshipChangeRequestMetadata Request { get; set; }
    public RelationshipChangeResponseMetadata? Response { get; set; }

    public required string Type { get; set; }

    public required string Status { get; set; }
}

public class RelationshipChangeRequestMetadata
{
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }
}

public class RelationshipChangeResponseMetadata
{
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }
}

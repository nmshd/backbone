namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;

public class Relationship
{
    public required string Id { get; set; }
    public required string RelationshipTemplateId { get; set; }

    public required string From { get; set; }
    public required string To { get; set; }
    public required List<RelationshipChange> Changes { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required string Status { get; set; }
}

public class RelationshipChange
{
    public required string Id { get; set; }

    public required string RelationshipId { get; set; }

    public required RelationshipChangeRequest Request { get; set; }
    public required RelationshipChangeResponse? Response { get; set; }

    public required string Type { get; set; }

    public required string Status { get; set; }
}

public class RelationshipChangeRequest
{
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }
    public required byte[]? Content { get; set; }
}

public class RelationshipChangeResponse
{
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }
    public required byte[]? Content { get; set; }
}

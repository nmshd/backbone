using Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.Entities;

public class RelationshipChange
{
    public string Id { get; }
    public string RelationshipId { get; }
    public Relationship Relationship { get; }

    public RelationshipChangeType Type { get; }
    public RelationshipChangeStatus Status { get; private set; }

    public DateTime CreatedAt { get; }

    public RelationshipChangeRequest Request { get; }
    public RelationshipChangeResponse? Response { get; private set; }

}

public class RelationshipChangeRequest
{
    public string Id { get; }
    public DateTime CreatedAt { get; }
    public IdentityAddress CreatedBy { get; }
    public DeviceId CreatedByDevice { get; }
    public byte[]? Content { get; private set; }

    public void LoadContent(byte[] content)
    {
        if (Content != null)
            throw new Exception("Cannot change the content of a relationship template.");

        Content = content;
    }
}

public class RelationshipChangeResponse
{
    public string Id { get; }
    public DateTime CreatedAt { get; }
    public IdentityAddress CreatedBy { get; }
    public DeviceId CreatedByDevice { get; }

    public byte[]? Content { get; private set; }

    public void LoadContent(byte[] content)
    {
        if (Content != null)
            throw new Exception("Cannot change the content of a relationship template.");

        Content = content;
    }
}

public enum RelationshipChangeType
{
    Creation = 10,
    Termination = 20,
    TerminationCancellation = 30
}

public enum RelationshipChangeStatus
{
    Pending = 10,
    Accepted = 20,
    Rejected = 30,
    Revoked = 40
}

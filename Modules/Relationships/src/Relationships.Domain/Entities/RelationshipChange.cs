using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Relationships.Domain.Errors;
using Relationships.Domain.Ids;

namespace Relationships.Domain.Entities;

public class RelationshipChange
{
#pragma warning disable CS8618
    protected RelationshipChange() { }
#pragma warning restore CS8618

    protected RelationshipChange(Relationship relationship, IdentityAddress createdBy, DeviceId createdByDevice, RelationshipChangeType type, byte[]? requestContent)
    {
        Id = RelationshipChangeId.New();
        RelationshipId = relationship.Id;
        Relationship = relationship;
        Type = type;
        Status = RelationshipChangeStatus.Pending;
        Request = new RelationshipChangeRequest(Id, createdBy, createdByDevice, requestContent);
        CreatedAt = SystemTime.UtcNow;
    }

    public RelationshipChangeId Id { get; }
    public RelationshipId RelationshipId { get; }
    public Relationship Relationship { get; }

    public RelationshipChangeType Type { get; }
    public RelationshipChangeStatus Status { get; private set; }
    public DateTime CreatedAt { get; }

    public RelationshipChangeRequest Request { get; }
    public RelationshipChangeResponse? Response { get; private set; }

    public bool IsCompleted => Status != RelationshipChangeStatus.Pending;

    internal void Accept(IdentityAddress by, DeviceId byDevice, byte[]? content = null)
    {
        EnsureCanBeAccepted(by, content);
        Status = RelationshipChangeStatus.Accepted;
        Response = new RelationshipChangeResponse(Id, by, byDevice, content);
    }

    protected virtual void EnsureCanBeAccepted(IdentityAddress by, byte[]? content)
    {
        if (IsCompleted)
            throw new DomainException(DomainErrors.ChangeRequestIsAlreadyCompleted(Status));

        if (by == Request.CreatedBy)
            throw new DomainException(DomainErrors.ChangeRequestCannotBeAcceptedByCreator());
    }

    internal virtual void Reject(IdentityAddress by, DeviceId byDevice, byte[]? content = null)
    {
        EnsureCanBeRejected(by, content);
        Status = RelationshipChangeStatus.Rejected;
        Response = new RelationshipChangeResponse(Id, by, byDevice, content);
    }

    protected virtual void EnsureCanBeRejected(IdentityAddress by, byte[]? content)
    {
        if (IsCompleted)
            throw new DomainException(DomainErrors.ChangeRequestIsAlreadyCompleted(Status));

        if (by == Request.CreatedBy)
            throw new DomainException(DomainErrors.ChangeRequestCannotBeAcceptedByCreator());
    }

    internal virtual void Revoke(IdentityAddress by, DeviceId byDevice, byte[]? content = null)
    {
        EnsureCanBeRevoked(by, content);
        Status = RelationshipChangeStatus.Revoked;
        Response = new RelationshipChangeResponse(Id, by, byDevice, content);
    }

    protected virtual void EnsureCanBeRevoked(IdentityAddress by, byte[]? content)
    {
        if (IsCompleted)
            throw new DomainException(DomainErrors.ChangeRequestIsAlreadyCompleted(Status));

        if (by != Request.CreatedBy)
            throw new DomainException(DomainErrors.ChangeRequestCannotBeAcceptedByCreator());
    }
}

public class RelationshipChangeRequest
{
#pragma warning disable CS8618
    private RelationshipChangeRequest() { }
#pragma warning restore CS8618

    public RelationshipChangeRequest(RelationshipChangeId changeId, IdentityAddress createdBy, DeviceId createdByDevice, byte[]? content = null)
    {
        Id = changeId;
        CreatedAt = SystemTime.UtcNow;
        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;
        Content = content;
    }

    public RelationshipChangeId Id { get; }
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
#pragma warning disable CS8618
    private RelationshipChangeResponse() { }
#pragma warning restore CS8618

    public RelationshipChangeResponse(RelationshipChangeId changeId, IdentityAddress createdBy, DeviceId createdByDevice, byte[]? content = null)
    {
        Id = changeId;
        CreatedAt = SystemTime.UtcNow;
        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;
        Content = content;
    }

    public RelationshipChangeId Id { get; }
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

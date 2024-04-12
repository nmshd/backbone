using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class Relationship
{
    // ReSharper disable once UnusedMember.Local
    private Relationship()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        RelationshipTemplateId = null!;
        RelationshipTemplate = null!;
        From = null!;
        To = null!;
        AuditLog = null!;
    }

    public Relationship(RelationshipTemplate relationshipTemplate, IdentityAddress activeIdentity, DeviceId activeDevice, byte[]? creationContent, List<Relationship> existingRelationships)
    {
        EnsureTargetIsNotSelf(relationshipTemplate, activeIdentity);
        EnsureNoActiveRelationshipToTargetExists(relationshipTemplate.CreatedBy, existingRelationships);

        Id = RelationshipId.New();
        RelationshipTemplateId = relationshipTemplate.Id;
        RelationshipTemplate = relationshipTemplate;

        From = activeIdentity;
        To = relationshipTemplate.CreatedBy;
        Status = RelationshipStatus.Pending;

        CreatedAt = SystemTime.UtcNow;

        CreationContent = creationContent;

        AuditLog = new List<RelationshipAuditLogEntry>
        {
            new(RelationshipAuditLogEntryReason.Creation, null, RelationshipStatus.Pending, activeIdentity, activeDevice)
        };
    }

    public RelationshipId Id { get; }
    public RelationshipTemplateId RelationshipTemplateId { get; }
    public RelationshipTemplate RelationshipTemplate { get; }

    public IdentityAddress From { get; }
    public IdentityAddress To { get; }

    public DateTime CreatedAt { get; }

    public RelationshipStatus Status { get; private set; }
    public byte[]? CreationContent { get; }
    public byte[]? AcceptanceContent { get; private set; }
    public List<RelationshipAuditLogEntry> AuditLog { get; }

    public IdentityAddress LastModifiedBy => AuditLog.Last().CreatedBy;

    private static void EnsureTargetIsNotSelf(RelationshipTemplate relationshipTemplate, IdentityAddress activeIdentity)
    {
        if (activeIdentity == relationshipTemplate.CreatedBy)
            throw new DomainException(DomainErrors.CannotSendRelationshipRequestToYourself());
    }

    private static void EnsureNoActiveRelationshipToTargetExists(IdentityAddress target, List<Relationship> existingRelationships)
    {
        if (existingRelationships.Any(r => r.Status == RelationshipStatus.Active))
            throw new DomainException(DomainErrors.RelationshipToTargetAlreadyExists(target));
    }

    public void Accept(IdentityAddress activeIdentity, DeviceId activeDevice, byte[]? acceptanceContent)
    {
        EnsureStatus(RelationshipStatus.Pending);
        EnsureRelationshipRequestIsAddressedToSelf(activeIdentity);

        Status = RelationshipStatus.Active;
        AcceptanceContent = acceptanceContent;

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.AcceptanceOfCreation,
            RelationshipStatus.Pending,
            RelationshipStatus.Active,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);
    }

    private void EnsureRelationshipRequestIsAddressedToSelf(IdentityAddress activeIdentity)
    {
        if (To != activeIdentity)
            throw new DomainException(DomainErrors.CannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse());
    }

    private void EnsureRelationshipRequestIsCreatedBySelf(IdentityAddress activeIdentity)
    {
        if (From != activeIdentity)
            throw new DomainException(DomainErrors.CannotRevokeRelationshipRequestNotCreatedByYourself());
    }

    public void Reject(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        EnsureStatus(RelationshipStatus.Pending);
        EnsureRelationshipRequestIsAddressedToSelf(activeIdentity);

        Status = RelationshipStatus.Rejected;

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.RejectionOfCreation,
            RelationshipStatus.Pending,
            RelationshipStatus.Rejected,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);
    }

    private void EnsureStatus(RelationshipStatus status)
    {
        if (Status != status)
            throw new DomainException(DomainErrors.RelationshipIsNotInCorrectStatus(status));
    }

    public void Revoke(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        EnsureStatus(RelationshipStatus.Pending);
        EnsureRelationshipRequestIsCreatedBySelf(activeIdentity);

        Status = RelationshipStatus.Revoked;

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.RevocationOfCreation,
            RelationshipStatus.Pending,
            RelationshipStatus.Revoked,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);
    }

    #region Expressions

    public static Expression<Func<Relationship, bool>> HasParticipant(string identity)
    {
        return r => r.From == identity || r.To == identity;
    }

    public static Expression<Func<Relationship, bool>> CountsAsActive()
    {
        return r => r.Status != RelationshipStatus.Rejected &&
                    r.Status != RelationshipStatus.Revoked;
    }

    #endregion
}

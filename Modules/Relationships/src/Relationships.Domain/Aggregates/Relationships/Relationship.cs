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
        EnsureNoOtherRelationshipToPeerExists(relationshipTemplate.CreatedBy, existingRelationships);

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
    public byte[]? CreationResponseContent { get; private set; }
    public List<RelationshipAuditLogEntry> AuditLog { get; }

    public IdentityAddress LastModifiedBy => AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedBy;

    private static void EnsureTargetIsNotSelf(RelationshipTemplate relationshipTemplate, IdentityAddress activeIdentity)
    {
        if (activeIdentity == relationshipTemplate.CreatedBy)
            throw new DomainException(DomainErrors.CannotSendRelationshipRequestToYourself());
    }

    private static void EnsureNoOtherRelationshipToPeerExists(IdentityAddress target, IEnumerable<Relationship> existingRelationshipsToPeer)
    {
        if (existingRelationshipsToPeer.Any(r => r.Status is RelationshipStatus.Active or RelationshipStatus.Pending or RelationshipStatus.Terminated))
            throw new DomainException(DomainErrors.RelationshipToTargetAlreadyExists(target));
    }

    public void Accept(IdentityAddress activeIdentity, DeviceId activeDevice, byte[]? creationResponseContent)
    {
        EnsureStatus(RelationshipStatus.Pending);
        EnsureRelationshipRequestIsAddressedToSelf(activeIdentity);

        Status = RelationshipStatus.Active;
        CreationResponseContent = creationResponseContent;

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.AcceptanceOfCreation,
            RelationshipStatus.Pending,
            RelationshipStatus.Active,
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

    private void EnsureRelationshipRequestIsAddressedToSelf(IdentityAddress activeIdentity)
    {
        if (To != activeIdentity)
            throw new DomainException(DomainErrors.CannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse());
    }

    public void Reject(IdentityAddress activeIdentity, DeviceId activeDevice, byte[]? creationResponseContent)
    {
        EnsureStatus(RelationshipStatus.Pending);
        EnsureRelationshipRequestIsAddressedToSelf(activeIdentity);

        CreationResponseContent = creationResponseContent;
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

    public void Revoke(IdentityAddress activeIdentity, DeviceId activeDevice, byte[]? creationResponseContent)
    {
        EnsureStatus(RelationshipStatus.Pending);
        EnsureRelationshipRequestIsCreatedBySelf(activeIdentity);

        CreationResponseContent = creationResponseContent;
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

    private void EnsureRelationshipRequestIsCreatedBySelf(IdentityAddress activeIdentity)
    {
        if (From != activeIdentity)
            throw new DomainException(DomainErrors.CannotRevokeRelationshipRequestNotCreatedByYourself());
    }

    public void Terminate(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        EnsureStatus(RelationshipStatus.Active);

        Status = RelationshipStatus.Terminated;

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.Termination,
            RelationshipStatus.Active,
            RelationshipStatus.Terminated,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);
    }

    public void RequestReactivation(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        EnsureThereIsNoOpenReactivationRequest();
        EnsureStatus(RelationshipStatus.Terminated);

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.ReactivationRequested,
            RelationshipStatus.Terminated,
            RelationshipStatus.Terminated,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);
    }

    private void EnsureThereIsNoOpenReactivationRequest()
    {
        var auditLogEntry = AuditLog.OrderBy(a => a.CreatedAt).Last();

        if (auditLogEntry.Reason == RelationshipAuditLogEntryReason.ReactivationRequested)
            throw new DomainException(DomainErrors.CannotRequestReactivationWhenThereIsAnOpenReactivationRequest());
    }

    public void RejectReactivation(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        EnsureRejectableRelationshipReactivationRequestExistsFor(activeIdentity);

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.RejectionOfReactivation,
            RelationshipStatus.Terminated,
            RelationshipStatus.Terminated,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);
    }

    private void EnsureRejectableRelationshipReactivationRequestExistsFor(IdentityAddress activeIdentity)
    {
        if (AuditLog.OrderBy(a => a.CreatedAt).Last().Reason != RelationshipAuditLogEntryReason.ReactivationRequested ||
            AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedBy == activeIdentity)
            throw new DomainException(DomainErrors.NoRejectableReactivationRequestExists());
    }

    public void RevokeReactivation(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        EnsureRevocableReactivationRequestExistsFor(activeIdentity);

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.RevocationOfReactivation,
            RelationshipStatus.Terminated,
            RelationshipStatus.Terminated,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);
    }

    private void EnsureRevocableReactivationRequestExistsFor(IdentityAddress activeIdentity)
    {
        if (AuditLog.OrderBy(a => a.CreatedAt).Last().Reason != RelationshipAuditLogEntryReason.ReactivationRequested ||
            AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedBy != activeIdentity)
            throw new DomainException(DomainErrors.NoRevocableReactivationRequestExists(activeIdentity));
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

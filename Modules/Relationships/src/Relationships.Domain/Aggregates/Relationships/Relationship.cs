using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class Relationship : Entity
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
        EnsureCanEstablish(relationshipTemplate, activeIdentity, existingRelationships);

        Id = RelationshipId.New();
        RelationshipTemplateId = relationshipTemplate.Id;
        RelationshipTemplate = relationshipTemplate;

        From = activeIdentity;
        To = relationshipTemplate.CreatedBy;
        Status = RelationshipStatus.Pending;

        CreatedAt = SystemTime.UtcNow;

        CreationContent = creationContent;

        AuditLog = [new(RelationshipAuditLogEntryReason.Creation, null, RelationshipStatus.Pending, activeIdentity, activeDevice)];

        RaiseDomainEvent(new RelationshipStatusChangedDomainEvent(this));
    }

    private void EnsureCanEstablish(RelationshipTemplate relationshipTemplate, IdentityAddress activeIdentity, List<Relationship> existingRelationships)
    {
        var error = CanEstablish(existingRelationships);

        if (error != null)
            throw new DomainException(error);

        EnsureTargetIsNotSelf(relationshipTemplate, activeIdentity);
    }

    public static DomainError? CanEstablish(IEnumerable<Relationship> existingRelationships)
    {
        if (AnotherRelationshipToPeerExists(existingRelationships))
            return DomainErrors.RelationshipToTargetAlreadyExists();

        return null;
    }

    private static bool AnotherRelationshipToPeerExists(IEnumerable<Relationship> existingRelationshipsToPeer)
    {
        return existingRelationshipsToPeer.Any(r => r.Status is RelationshipStatus.Active or RelationshipStatus.Pending or RelationshipStatus.Terminated or RelationshipStatus.DeletionProposed);
    }

    private static void EnsureTargetIsNotSelf(RelationshipTemplate relationshipTemplate, IdentityAddress activeIdentity)
    {
        if (activeIdentity == relationshipTemplate.CreatedBy)
            throw new DomainException(DomainErrors.CannotSendRelationshipRequestToYourself());
    }

    public RelationshipId Id { get; }
    public RelationshipTemplateId? RelationshipTemplateId { get; }
    public RelationshipTemplate? RelationshipTemplate { get; }

    public IdentityAddress From { get; private set; }
    public IdentityAddress To { get; private set; }

    public DateTime CreatedAt { get; }

    public RelationshipStatus Status { get; private set; }
    public byte[]? CreationContent { get; }
    public byte[]? CreationResponseContent { get; private set; }
    public List<RelationshipAuditLogEntry> AuditLog { get; }

    public IdentityAddress LastModifiedBy => AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedBy;

    public bool FromHasDecomposed { get; private set; }
    public bool ToHasDecomposed { get; private set; }

    public IdentityAddress GetPeerOf(IdentityAddress activeIdentity)
    {
        return From == activeIdentity ? To : From;
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

        RaiseDomainEvent(new RelationshipStatusChangedDomainEvent(this));
    }

    private void EnsureStatus(params RelationshipStatus[] statuses)
    {
        if (!statuses.Contains(Status))
            throw new DomainException(DomainErrors.RelationshipIsNotInCorrectStatus(statuses));
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

        RaiseDomainEvent(new RelationshipStatusChangedDomainEvent(this));
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

        RaiseDomainEvent(new RelationshipStatusChangedDomainEvent(this));
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

        RaiseDomainEvent(new RelationshipStatusChangedDomainEvent(this));
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

        RaiseDomainEvent(new RelationshipReactivationRequestedDomainEvent(this, activeIdentity, GetPeerOf(activeIdentity)));
    }

    private void EnsureThereIsNoOpenReactivationRequest()
    {
        var auditLogEntry = AuditLog.OrderBy(a => a.CreatedAt).Last();

        if (auditLogEntry.Reason == RelationshipAuditLogEntryReason.ReactivationRequested)
            throw new DomainException(DomainErrors.CannotRequestReactivationWhenThereIsAnOpenReactivationRequest());
    }

    public void AcceptReactivation(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        EnsureAcceptableReactivationRequestExistsFor(activeIdentity);

        Status = RelationshipStatus.Active;

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.AcceptanceOfReactivation,
            RelationshipStatus.Terminated,
            RelationshipStatus.Active,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);

        RaiseDomainEvent(new RelationshipReactivationCompletedDomainEvent(this, GetPeerOf(activeIdentity)));
    }

    private void EnsureAcceptableReactivationRequestExistsFor(IdentityAddress activeIdentity)
    {
        if (AuditLog.OrderBy(a => a.CreatedAt).Last().Reason != RelationshipAuditLogEntryReason.ReactivationRequested ||
            AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedBy == activeIdentity)
            throw new DomainException(DomainErrors.NoAcceptableRelationshipReactivationRequestExists());
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

        RaiseDomainEvent(new RelationshipReactivationCompletedDomainEvent(this, GetPeerOf(activeIdentity)));
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

        RaiseDomainEvent(new RelationshipReactivationCompletedDomainEvent(this, GetPeerOf(activeIdentity)));
    }

    private void EnsureRevocableReactivationRequestExistsFor(IdentityAddress activeIdentity)
    {
        if (AuditLog.OrderBy(a => a.CreatedAt).Last().Reason != RelationshipAuditLogEntryReason.ReactivationRequested ||
            AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedBy != activeIdentity)
            throw new DomainException(DomainErrors.NoRevocableReactivationRequestExists());
    }

    public void Decompose(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        EnsureHasParticipant(activeIdentity);
        EnsureIsNotDecomposedBy(activeIdentity);
        EnsureStatus(RelationshipStatus.Rejected, RelationshipStatus.Revoked, RelationshipStatus.Terminated, RelationshipStatus.DeletionProposed);

        if (Status is RelationshipStatus.Terminated or RelationshipStatus.Rejected or RelationshipStatus.Revoked)
            DecomposeAsFirstParticipant(activeIdentity, activeDevice, RelationshipAuditLogEntryReason.Decomposition);
        else
            DecomposeAsSecondParticipant(activeIdentity, activeDevice, RelationshipAuditLogEntryReason.Decomposition);

        RaiseDomainEvent(new RelationshipStatusChangedDomainEvent(this));
    }

    public void DecomposeDueToIdentityDeletion(IdentityAddress identityToBeDeleted, string didDomainName)
    {
        EnsureHasParticipant(identityToBeDeleted);

        var peer = GetPeerOf(identityToBeDeleted);

        if (From == identityToBeDeleted && FromHasDecomposed || To == identityToBeDeleted && ToHasDecomposed)
            return;

        if (Status is RelationshipStatus.DeletionProposed)
            DecomposeAsSecondParticipant(identityToBeDeleted, null, RelationshipAuditLogEntryReason.DecompositionDueToIdentityDeletion);
        else
            DecomposeAsFirstParticipant(identityToBeDeleted, null, RelationshipAuditLogEntryReason.DecompositionDueToIdentityDeletion);

        AnonymizeParticipant(identityToBeDeleted, didDomainName);
        RaiseDomainEvent(new RelationshipStatusChangedDomainEvent(Id, Status.ToString(), identityToBeDeleted, peer));
    }

    private void DecomposeAsFirstParticipant(IdentityAddress activeIdentity, DeviceId? activeDevice, RelationshipAuditLogEntryReason reason)
    {
        var oldStatus = Status;

        Status = RelationshipStatus.DeletionProposed;

        var auditLogEntry = new RelationshipAuditLogEntry(
            reason,
            oldStatus,
            RelationshipStatus.DeletionProposed,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);

        if (From == activeIdentity)
            FromHasDecomposed = true;
        else
            ToHasDecomposed = true;
    }

    private void DecomposeAsSecondParticipant(IdentityAddress activeIdentity, DeviceId? activeDevice, RelationshipAuditLogEntryReason reason)
    {
        EnsureStatus(RelationshipStatus.DeletionProposed);

        Status = RelationshipStatus.ReadyForDeletion;

        var auditLogEntry = new RelationshipAuditLogEntry(
            reason,
            RelationshipStatus.DeletionProposed,
            RelationshipStatus.ReadyForDeletion,
            activeIdentity,
            activeDevice
        );
        AuditLog.Add(auditLogEntry);

        if (From == activeIdentity)
            FromHasDecomposed = true;
        else
            ToHasDecomposed = true;
    }

    public void EnsureIsNotDecomposedBy(IdentityAddress activeIdentity)
    {
        if (From == activeIdentity && FromHasDecomposed || To == activeIdentity && ToHasDecomposed)
            throw new DomainException(DomainErrors.RelationshipAlreadyDecomposed());
    }

    private void EnsureHasParticipant(IdentityAddress activeIdentity)
    {
        if (From != activeIdentity && To != activeIdentity)
            throw new DomainException(DomainErrors.RequestingIdentityDoesNotBelongToRelationship());
    }

    private void AnonymizeParticipant(IdentityAddress identityToAnonymize, string didDomainName)
    {
        EnsureHasParticipant(identityToAnonymize);
        EnsureStatus(RelationshipStatus.DeletionProposed, RelationshipStatus.ReadyForDeletion);

        var anonymousIdentity = IdentityAddress.GetAnonymized(didDomainName);

        if (From == identityToAnonymize)
            From = anonymousIdentity;
        else
            To = anonymousIdentity;

        foreach (var auditLogEntry in AuditLog)
            if (auditLogEntry.CreatedBy == identityToAnonymize)
                auditLogEntry.AnonymizeIdentity(anonymousIdentity);
    }

    #region Expressions

    public static Expression<Func<Relationship, bool>> IsBetween(string identity1, string identity2)
    {
        return r => r.From == identity1 && r.To == identity2 ||
                    r.From == identity2 && r.To == identity1;
    }

    public static Expression<Func<Relationship, bool>> HasParticipant(string identity)
    {
        return r => r.From == identity || r.To == identity;
    }

    public static Expression<Func<Relationship, bool>> CountsAsActive()
    {
        return r => r.Status != RelationshipStatus.Rejected &&
                    r.Status != RelationshipStatus.Revoked &&
                    r.Status != RelationshipStatus.ReadyForDeletion;
    }

    public static Expression<Func<Relationship, bool>> HasStatusInWhichPeerShouldBeNotifiedAboutDeletion()
    {
        return r => r.Status == RelationshipStatus.Pending ||
                    r.Status == RelationshipStatus.Active ||
                    r.Status == RelationshipStatus.Terminated;
    }

    public static Expression<Func<Relationship, bool>> HasStatusInWhichPeerShouldBeNotifiedAboutFeatureFlagsChange()
    {
        return r => r.Status == RelationshipStatus.Pending ||
                    r.Status == RelationshipStatus.Active;
    }

    #endregion
}

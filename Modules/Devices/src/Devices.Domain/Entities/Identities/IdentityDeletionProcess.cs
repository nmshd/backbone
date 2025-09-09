using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcess : Entity
{
    private readonly List<IdentityDeletionProcessAuditLogEntry> _auditLog;

    // ReSharper disable once UnusedMember.Local
    protected IdentityDeletionProcess()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        IdentityAddress = null!;
        _auditLog = null!;
        Id = null!;
    }

    private IdentityDeletionProcess(IdentityAddress createdBy, DeletionProcessStatus status)
    {
        Id = IdentityDeletionProcessId.Generate();
        IdentityAddress = null!;
        CreatedAt = SystemTime.UtcNow;
        Status = status;

        _auditLog = [IdentityDeletionProcessAuditLogEntry.ProcessStartedBySupport(createdBy)];

        RaiseDomainEvent(new IdentityDeletionProcessStartedDomainEvent(createdBy, Id, null));
    }

    private IdentityDeletionProcess(IdentityAddress createdBy, DeviceId createdByDevice, double? lengthOfGracePeriodInDays)
    {
        Id = IdentityDeletionProcessId.Generate();
        IdentityAddress = null!;
        CreatedAt = SystemTime.UtcNow;

        ApproveInternally(createdBy, createdByDevice, lengthOfGracePeriodInDays);

        _auditLog = [IdentityDeletionProcessAuditLogEntry.ProcessStartedByOwner(createdBy, createdByDevice)];
    }

    public IdentityDeletionProcessId Id { get; }

    private IdentityAddress IdentityAddress { get; }

    public virtual IReadOnlyList<IdentityDeletionProcessAuditLogEntry> AuditLog => _auditLog;
    public DeletionProcessStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime ApprovalPeriodEndsAt => CreatedAt.AddDays(IdentityDeletionConfiguration.Instance.LengthOfApprovalPeriodInDays);

    public DateTime? ApprovalReminder1SentAt { get; private set; }
    public DateTime? ApprovalReminder2SentAt { get; private set; }
    public DateTime? ApprovalReminder3SentAt { get; private set; }

    public DateTime? ApprovedAt { get; private set; }
    public DeviceId? ApprovedByDevice { get; private set; }

    public DateTime? RejectedAt { get; private set; }
    public DeviceId? RejectedByDevice { get; private set; }

    public DateTime? CancelledAt { get; private set; }
    public DeviceId? CancelledByDevice { get; private set; }

    public DateTime? GracePeriodEndsAt { get; private set; }

    public DateTime? GracePeriodReminder1SentAt { get; private set; }
    public DateTime? GracePeriodReminder2SentAt { get; private set; }
    public DateTime? GracePeriodReminder3SentAt { get; private set; }

    public DateTime? DeletionStartedAt { get; private set; }

    public bool HasApprovalPeriodExpired => Status == DeletionProcessStatus.WaitingForApproval && SystemTime.UtcNow >= ApprovalPeriodEndsAt;

    public bool HasGracePeriodExpired => Status == DeletionProcessStatus.Approved && SystemTime.UtcNow >= GracePeriodEndsAt;

    public static Expression<Func<IdentityDeletionProcess, bool>> CanBeCleanedUp =>
        p => p.Status == DeletionProcessStatus.Cancelled && p.CancelledAt!.Value.AddDays(30) < SystemTime.UtcNow ||
             p.Status == DeletionProcessStatus.Rejected && p.RejectedAt!.Value.AddDays(30) < SystemTime.UtcNow;

    public static IdentityDeletionProcess StartAsSupport(IdentityAddress createdBy)
    {
        return new IdentityDeletionProcess(createdBy, DeletionProcessStatus.WaitingForApproval);
    }

    public static IdentityDeletionProcess StartAsOwner(IdentityAddress createdBy, DeviceId createdByDeviceId, double? lengthOfGracePeriodInDays)
    {
        return new IdentityDeletionProcess(createdBy, createdByDeviceId, lengthOfGracePeriodInDays);
    }

    public bool IsActive()
    {
        return Status is DeletionProcessStatus.Approved or DeletionProcessStatus.WaitingForApproval or DeletionProcessStatus.Deleting;
    }

    public void ApprovalReminder1Sent(IdentityAddress address)
    {
        ApprovalReminder1SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ApprovalReminder1Sent(address));
    }

    public void ApprovalReminder2Sent(IdentityAddress address)
    {
        ApprovalReminder2SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ApprovalReminder2Sent(address));
    }

    public void ApprovalReminder3Sent(IdentityAddress address)
    {
        ApprovalReminder3SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ApprovalReminder3Sent(address));
    }

    public void GracePeriodReminder1Sent(IdentityAddress address)
    {
        GracePeriodReminder1SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.GracePeriodReminder1Sent(address));
    }

    public void GracePeriodReminder2Sent(IdentityAddress address)
    {
        GracePeriodReminder2SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.GracePeriodReminder2Sent(address));
    }

    public void GracePeriodReminder3Sent(IdentityAddress address)
    {
        GracePeriodReminder3SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.GracePeriodReminder3Sent(address));
    }

    internal void DeletionStarted(IdentityAddress address)
    {
        EnsureStatus(DeletionProcessStatus.Approved);
        EnsureGracePeriodHasExpired();

        ChangeStatus(DeletionProcessStatus.Deleting, address, address);
        DeletionStartedAt = SystemTime.UtcNow;
    }

    private void EnsureGracePeriodHasExpired()
    {
        if (!HasGracePeriodExpired)
            throw new DomainException(DomainErrors.GracePeriodHasNotYetExpired());
    }

    public void Approve(IdentityAddress address, DeviceId approvedByDevice)
    {
        EnsureStatus(DeletionProcessStatus.WaitingForApproval);

        ApproveInternally(address, approvedByDevice);
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ProcessApproved(address, approvedByDevice));
    }

    private void ApproveInternally(IdentityAddress address, DeviceId createdByDevice, double? lengthOfGracePeriodInDays = null)
    {
        lengthOfGracePeriodInDays ??= IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays;
        ApprovedAt = SystemTime.UtcNow;
        ApprovedByDevice = createdByDevice;
        GracePeriodEndsAt = SystemTime.UtcNow.AddDays(lengthOfGracePeriodInDays.Value);
        ChangeStatus(DeletionProcessStatus.Approved, address, address);
    }

    public void Reject(IdentityAddress address, DeviceId rejectedByDevice)
    {
        EnsureStatus(DeletionProcessStatus.WaitingForApproval);

        ChangeStatus(DeletionProcessStatus.Rejected, address, address);
        RejectedAt = SystemTime.UtcNow;
        RejectedByDevice = rejectedByDevice;

        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ProcessRejected(address, rejectedByDevice));
    }

    public void EnsureStatus(DeletionProcessStatus deletionProcessStatus)
    {
        if (Status != deletionProcessStatus)
            throw new DomainException(DomainErrors.DeletionProcessMustBeInStatus(deletionProcessStatus));
    }

    public void CancelAsOwner(IdentityAddress address, DeviceId cancelledByDevice)
    {
        if (Status != DeletionProcessStatus.Approved)
            throw new DomainException(DomainErrors.DeletionProcessMustBeInStatus(DeletionProcessStatus.Approved));

        if (GracePeriodEndsAt < SystemTime.UtcNow)
            throw new DomainException(DomainErrors.GracePeriodHasAlreadyExpired());

        ChangeStatus(DeletionProcessStatus.Cancelled, address, address);
        CancelledAt = SystemTime.UtcNow;
        CancelledByDevice = cancelledByDevice;
        GracePeriodEndsAt = null;

        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ProcessCancelledByOwner(address, cancelledByDevice));
    }

    public void CancelAsSupport(IdentityAddress address)
    {
        if (Status != DeletionProcessStatus.Approved)
            throw new DomainException(DomainErrors.DeletionProcessMustBeInStatus(DeletionProcessStatus.Approved));

        ChangeStatus(DeletionProcessStatus.Cancelled, address, null);
        CancelledAt = SystemTime.UtcNow;

        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ProcessCancelledBySupport(address));
    }

    public void Cancel(IdentityAddress address)
    {
        EnsureStatus(DeletionProcessStatus.WaitingForApproval);

        ChangeStatus(DeletionProcessStatus.Cancelled, address, null);
        CancelledAt = SystemTime.UtcNow;

        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ProcessCancelledAutomatically(address));
    }

    public void ErrorDuringDeletion(IdentityAddress address, string errorMessage)
    {
        EnsureStatus(DeletionProcessStatus.Deleting);

        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ErrorDuringDeletion(address, errorMessage));
    }

    private void ChangeStatus(DeletionProcessStatus newStatus, IdentityAddress address, IdentityAddress? initiator)
    {
        Status = newStatus;
        RaiseDomainEvent(new IdentityDeletionProcessStatusChangedDomainEvent(address, Id, initiator?.Value));
    }
}

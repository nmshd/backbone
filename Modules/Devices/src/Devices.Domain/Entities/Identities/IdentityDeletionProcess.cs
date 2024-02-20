﻿using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcess
{
    private readonly List<IdentityDeletionProcessAuditLogEntry> _auditLog;

    // ReSharper disable once UnusedMember.Local
    private IdentityDeletionProcess()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        _auditLog = null!;
        Id = null!;
    }

    public static IdentityDeletionProcess StartAsSupport(IdentityAddress createdBy)
    {
        return new IdentityDeletionProcess(createdBy, DeletionProcessStatus.WaitingForApproval);
    }

    public static IdentityDeletionProcess StartAsOwner(IdentityAddress createdBy, DeviceId createdByDeviceId)
    {
        return new IdentityDeletionProcess(createdBy, createdByDeviceId);
    }

    private IdentityDeletionProcess(IdentityAddress createdBy, DeletionProcessStatus status)
    {
        Id = IdentityDeletionProcessId.Generate();
        CreatedAt = SystemTime.UtcNow;
        Status = status;

        _auditLog = new List<IdentityDeletionProcessAuditLogEntry>
        {
            IdentityDeletionProcessAuditLogEntry.ProcessStartedBySupport(Id, createdBy)
        };
    }

    private IdentityDeletionProcess(IdentityAddress createdBy, DeviceId createdByDevice)
    {
        Id = IdentityDeletionProcessId.Generate();
        CreatedAt = SystemTime.UtcNow;

        Approve(createdByDevice);

        _auditLog = new List<IdentityDeletionProcessAuditLogEntry>
        {
            IdentityDeletionProcessAuditLogEntry.ProcessStartedByOwner(Id, createdBy, createdByDevice)
        };
    }

    private void Approve(DeviceId createdByDevice)
    {
        Status = DeletionProcessStatus.Approved;
        ApprovedAt = SystemTime.UtcNow;
        ApprovedByDevice = createdByDevice;
        GracePeriodEndsAt = SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.LengthOfGracePeriod);
    }

    public IdentityDeletionProcessId Id { get; }
    public IReadOnlyList<IdentityDeletionProcessAuditLogEntry> AuditLog => _auditLog;
    public DeletionProcessStatus Status { get; private set; }
    public DateTime CreatedAt { get; }

    public DateTime? ApprovalReminder1SentAt { get; private set; }
    public DateTime? ApprovalReminder2SentAt { get; private set; }
    public DateTime? ApprovalReminder3SentAt { get; private set; }

    public DateTime? ApprovedAt { get; private set; }
    public DeviceId? ApprovedByDevice { get; private set; }

    public DateTime? GracePeriodEndsAt { get; private set; }

    public DateTime? GracePeriodReminder1SentAt { get; private set; }
    public DateTime? GracePeriodReminder2SentAt { get; private set; }
    public DateTime? GracePeriodReminder3SentAt { get; private set; }


    public bool IsActive()
    {
        return Status is DeletionProcessStatus.Approved or DeletionProcessStatus.WaitingForApproval;
    }

    public DateTime GetEndOfApprovalPeriod()
    {
        return CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime);
    }

    public void ApprovalReminder1Sent(IdentityAddress address)
    {
        ApprovalReminder1SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ApprovalReminder1Sent(Id, address));
    }

    public void ApprovalReminder2Sent(IdentityAddress address)
    {
        ApprovalReminder2SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ApprovalReminder2Sent(Id, address));
    }

    public void ApprovalReminder3Sent(IdentityAddress address)
    {
        ApprovalReminder3SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ApprovalReminder3Sent(Id, address));
    }

    public void GracePeriodReminder1Sent(IdentityAddress address)
    {
        GracePeriodReminder1SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.GracePeriodReminder1Sent(Id, address));
    }

    public void GracePeriodReminder2Sent(IdentityAddress address)
    {
        GracePeriodReminder2SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.GracePeriodReminder2Sent(Id, address));
    }

    public void GracePeriodReminder3Sent(IdentityAddress address)
    {
        GracePeriodReminder3SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.GracePeriodReminder3Sent(Id, address));
    }

    public void Approve(IdentityAddress address, DeviceId approvedByDevice)
    {
        if (Status != DeletionProcessStatus.WaitingForApproval)
            throw new DomainException(DomainErrors.NoDeletionProcessWithRequiredStatusExists());

        Approve(approvedByDevice);
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ProcessApproved(Id, address, approvedByDevice));
    }
}

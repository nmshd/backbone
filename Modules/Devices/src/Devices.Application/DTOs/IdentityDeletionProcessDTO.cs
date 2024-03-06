using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.DTOs;

public class IdentityDeletionProcessDTO
{
    public IdentityDeletionProcessDTO(IdentityDeletionProcess process)
    {
        Id = process.Id;
        AuditLog = process.AuditLog
            .Select(e => new IdentityDeletionProcessAuditLogEntryDTO(e))
            .ToList();
        Status = process.Status;
        CreatedAt = process.CreatedAt;
        ApprovalReminder1SentAt = process.ApprovalReminder1SentAt;
        ApprovalReminder2SentAt = process.ApprovalReminder2SentAt;
        ApprovalReminder3SentAt = process.ApprovalReminder3SentAt;
        ApprovedAt = process.ApprovedAt;
        ApprovedByDevice = process.ApprovedByDevice;
        GracePeriodEndsAt = process.GracePeriodEndsAt;
        GracePeriodReminder1SentAt = process.GracePeriodReminder1SentAt;
        GracePeriodReminder2SentAt = process.GracePeriodReminder2SentAt;
        GracePeriodReminder3SentAt = process.GracePeriodReminder3SentAt;
    }

    public string Id { get; set; }
    public List<IdentityDeletionProcessAuditLogEntryDTO> AuditLog { get; set; }
    public DeletionProcessStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? ApprovalReminder1SentAt { get; set; }
    public DateTime? ApprovalReminder2SentAt { get; set; }
    public DateTime? ApprovalReminder3SentAt { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public DeviceId? ApprovedByDevice { get; set; }

    public DateTime? GracePeriodEndsAt { get; set; }

    public DateTime? GracePeriodReminder1SentAt { get; set; }
    public DateTime? GracePeriodReminder2SentAt { get; set; }
    public DateTime? GracePeriodReminder3SentAt { get; set; }
}

public class IdentityDeletionProcessAuditLogEntryDTO
{
    public IdentityDeletionProcessAuditLogEntryDTO(IdentityDeletionProcessAuditLogEntry entry)
    {
        Id = entry.Id;
        CreatedAt = entry.CreatedAt;
        Message = entry.Message;
        OldStatus = entry.OldStatus;
        NewStatus = entry.NewStatus;
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Message { get; set; }
    public DeletionProcessStatus? OldStatus { get; set; }
    public DeletionProcessStatus NewStatus { get; set; }
}

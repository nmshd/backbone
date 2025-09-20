using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.DTOs;

public class IdentityDeletionProcessDetailsDTO
{
    public IdentityDeletionProcessDetailsDTO(IdentityDeletionProcess process)
    {
        Id = process.Id;
        AuditLog = process.AuditLog
            .Select(e => new IdentityDeletionProcessAuditLogEntryDTO(e))
            .OrderBy(e => e.CreatedAt)
            .ToList();
        Status = process.Status.ToString();
        CreatedAt = process.CreatedAt;

        ApprovedAt = process.ApprovedAt;
        ApprovedByDevice = process.ApprovedByDevice?.Value;

        GracePeriodEndsAt = process.GracePeriodEndsAt;

        GracePeriodReminder1SentAt = process.GracePeriodReminder1SentAt;
        GracePeriodReminder2SentAt = process.GracePeriodReminder2SentAt;
        GracePeriodReminder3SentAt = process.GracePeriodReminder3SentAt;
    }

    public string Id { get; set; }
    public List<IdentityDeletionProcessAuditLogEntryDTO> AuditLog { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedByDevice { get; set; }

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
        OldStatus = entry.OldStatus.ToString();
        NewStatus = entry.NewStatus.ToString();
        MessageKey = entry.MessageKey;
        AdditionalData = entry.AdditionalData ?? [];
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; }
    public MessageKey MessageKey { get; set; }
}

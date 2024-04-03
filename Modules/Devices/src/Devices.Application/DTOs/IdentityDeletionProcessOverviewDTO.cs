using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.DTOs;

public class IdentityDeletionProcessOverviewDTO
{
    public IdentityDeletionProcessOverviewDTO(IdentityDeletionProcess process)
    {
        Id = process.Id;
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

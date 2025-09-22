using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.DTOs;

public class IdentityDeletionProcessOverviewDTO
{
    public IdentityDeletionProcessOverviewDTO(IdentityDeletionProcess process)
    {
        Id = process.Id;
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
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedByDevice { get; set; }

    public DateTime? GracePeriodEndsAt { get; set; }

    public DateTime? GracePeriodReminder1SentAt { get; set; }
    public DateTime? GracePeriodReminder2SentAt { get; set; }
    public DateTime? GracePeriodReminder3SentAt { get; set; }
}

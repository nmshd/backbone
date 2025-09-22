using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.DTOs;

public class IdentityDeletionProcessOverviewDTO
{
    public IdentityDeletionProcessOverviewDTO(IdentityDeletionProcess process)
    {
        Id = process.Id;
        Status = process.Status.ToString();
        CreatedAt = process.CreatedAt;
        CreatedByDevice = process.CreatedByDevice;

        GracePeriodEndsAt = process.GracePeriodEndsAt;

        GracePeriodReminder1SentAt = process.GracePeriodReminder1SentAt;
        GracePeriodReminder2SentAt = process.GracePeriodReminder2SentAt;
        GracePeriodReminder3SentAt = process.GracePeriodReminder3SentAt;
    }

    public string Id { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByDevice { get; set; }

    [Obsolete(
        "There is no reason to use this property anymore since the possibility to start a deletion process as support was removed. It only stays for backwards compatibility reasons and will be removed in the future. Use `createdAt` instead")]
    public DateTime ApprovedAt => CreatedAt;

    [Obsolete(
        "There is no reason to use this property anymore since the possibility to start a deletion process as support was removed. It only stays for backwards compatibility reasons and will be removed in the future. Use `CreatedByDevice` instead")]
    public string ApprovedByDevice => CreatedByDevice;

    public DateTime? GracePeriodEndsAt { get; set; }

    public DateTime? GracePeriodReminder1SentAt { get; set; }
    public DateTime? GracePeriodReminder2SentAt { get; set; }
    public DateTime? GracePeriodReminder3SentAt { get; set; }
}

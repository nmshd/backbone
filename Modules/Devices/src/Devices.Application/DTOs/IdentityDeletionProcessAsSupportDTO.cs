using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.DTOs;

public class IdentityDeletionProcessAsSupportDTO
{
    public IdentityDeletionProcessAsSupportDTO(IdentityDeletionProcess process)
    {
        Id = process.Id;
        Status = process.Status;
        CreatedAt = process.CreatedAt;
        ApprovedAt = process.ApprovedAt;
        ApprovedByDevice = process.ApprovedByDevice;
        GracePeriodEndsAt = process.GracePeriodEndsAt;

    }

    public string Id { get; set; }
    public DeletionProcessStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DeviceId? ApprovedByDevice { get; set; }
    public DateTime? GracePeriodEndsAt { get; set; }
}

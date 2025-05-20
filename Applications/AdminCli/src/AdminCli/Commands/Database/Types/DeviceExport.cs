using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.AdminCli.Commands.Database.Types;

public class DeviceExport
{
    public required string DeviceId { get; set; } = null!;
    public required DateTime? LastLoginAt { get; set; }
    public required string IdentityAddress { get; set; } = null!;
    public required DateTime CreatedAt { get; set; }
    public required string Tier { get; set; } = null!;
    public required IdentityStatus IdentityStatus { get; set; }
    public required DateTime? IdentityDeletionGracePeriodEndsAt { get; set; }

    public string? ClientName { get; set; }
    public required PushNotificationPlatform? Platform { get; set; } = null!;
}

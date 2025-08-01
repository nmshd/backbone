using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;

namespace Backbone.Modules.Devices.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public required DatabaseConfiguration SqlDatabase { get; init; }

    [Required]
    public required PushNotificationConfiguration PushNotifications { get; init; }
}

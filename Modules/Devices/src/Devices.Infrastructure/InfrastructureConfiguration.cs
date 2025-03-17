using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;

namespace Backbone.Modules.Devices.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public DatabaseConfiguration SqlDatabase { get; set; } = new();

    [Required]
    public PushNotificationOptions PushNotifications { get; set; } = new();
}

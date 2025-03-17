using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using IServiceCollectionExtensions = Backbone.Modules.Devices.Infrastructure.Persistence.IServiceCollectionExtensions;

namespace Backbone.Modules.Devices.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public IServiceCollectionExtensions.DbOptions SqlDatabase { get; set; } = new();

    [Required]
    public PushNotificationOptions PushNotifications { get; set; } = new();
}

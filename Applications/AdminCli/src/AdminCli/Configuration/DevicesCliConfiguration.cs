using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;

namespace Backbone.AdminCli.Configuration;

public class DevicesCliConfiguration
{
    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    public class InfrastructureConfiguration
    {
        [Required]
        public PushNotificationOptions PushNotifications { get; set; } = new();
    }
}

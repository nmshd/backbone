using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Dummy;

public class DummyOptions
{
    [Required]
    public required bool Enabled { get; set; }
}

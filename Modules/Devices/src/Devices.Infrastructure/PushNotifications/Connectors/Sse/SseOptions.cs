using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public class SseOptions
{
    [Required]
    public required bool Enabled { get; set; }

    [Required]
    public bool EnableHealthCheck { get; set; } = true;

    [Url]
    [Required]
    public required string SseServerBaseAddress { get; set; }
}

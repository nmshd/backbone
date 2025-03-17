using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public class SseOptions
{
    [Required]
    public required bool Enabled { get; set; }

    public required bool EnableHealthCheck { get; set; } = false;

    [Url]
    [RequiredIf(nameof(Enabled), true)]
    public required string SseServerBaseAddress { get; set; }
}

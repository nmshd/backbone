using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public class SseConfiguration
{
    [Required]
    public required bool Enabled { get; init; }

    public bool EnableHealthCheck { get; init; } = false;

    [Url]
    [RequiredIf(nameof(Enabled), true)]
    public required string SseServerBaseAddress { get; init; }
}

using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public class SseOptions
{
    [Required]
    public required bool Enabled { get; set; }

    [Url]
    [Required]
    public required string SseServerBaseAddress { get; set; }
}

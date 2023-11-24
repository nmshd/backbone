using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class RegisterDeviceResponse
{
    public DeviceId? Id { get; set; }
    public string? Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public DeviceId? CreatedByDevice { get; set; }
}

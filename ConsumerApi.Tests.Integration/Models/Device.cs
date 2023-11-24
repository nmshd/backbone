using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class Device
{
    public DeviceId? Id { get; set; }

    public string? Username { get; set; }

    public DateTime CreatedAt { get; set; }

    public DeviceId? CreatedByDevice { get; set; }

    public LastLoginInformation? LastLogin { get; set; }
}

public class LastLoginInformation
{
    public DateTime? Time { get; set; }
}

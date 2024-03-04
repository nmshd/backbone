namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class Device
{
    public required string Id { get; set; }

    public required string Username { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required string CreatedByDevice { get; set; }

    public required LastLoginInformation LastLogin { get; set; }
}

public class LastLoginInformation
{
    public DateTime? Time { get; set; }
}

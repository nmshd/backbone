namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class CreateIdentityResponse
{
    public required string Address { get; set; }
    public DateTime CreatedAt { get; set; }

    public required CreateIdentityResponseDevice Device { get; set; }
}

public class CreateIdentityResponseDevice
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public DateTime CreatedAt { get; set; }
}


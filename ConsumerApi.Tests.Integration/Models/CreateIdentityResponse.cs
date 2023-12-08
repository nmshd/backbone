namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class CreateIdentityResponse
{
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }

    public CreateIdentityResponseDevice Device { get; set; }
}

public class CreateIdentityResponseDevice
{
    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
}


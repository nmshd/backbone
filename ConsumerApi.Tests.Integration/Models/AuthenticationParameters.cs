namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class AuthenticationParameters
{
    public string GrantType { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

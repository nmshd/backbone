namespace ConsumerApi.Tests.Integration.Tokens.Models;
public class AuthenticationParameters
{
    public string GrantType { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

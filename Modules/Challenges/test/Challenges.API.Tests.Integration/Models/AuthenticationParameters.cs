namespace Challenges.API.Tests.Integration.Models;
public class AuthenticationParameters
{
    public string GrantType { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

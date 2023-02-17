namespace Challenges.API.Client.Models;
public class AuthenticationRequest
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

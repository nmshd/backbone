namespace Challenges.API.Tests.Integration.Models;
public class RequestConfiguration
{
    public AuthenticationParameters AuthenticationParameters { get; set; } = new AuthenticationParameters();
    public bool Authenticate { get; set; } = false;
    public string ContentType { get; set; } = string.Empty;
    public string AcceptHeader { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

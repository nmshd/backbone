namespace Challenges.API.Tests.Integration.Models;
public class RequestConfiguration
{
    public AuthenticationParameters AuthenticationParameters { get; set; } = new AuthenticationParameters();
    public bool IsAuthenticated { get; set; } = false;
    public string ContentType { get; set; } = string.Empty;
    public string AcceptHeader { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

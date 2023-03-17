namespace Challenges.API.Tests.Integration.Models;
public class RequestConfiguration
{
    public AuthenticationParameters AuthenticationParameters { get; set; } = new AuthenticationParameters();
    public bool Authenticate { get; set; } = false;
    public string ContentType { get; set; } = string.Empty;
    public string AcceptHeader { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public void SupplementWith(RequestConfiguration other)
    {
        AcceptHeader = string.IsNullOrEmpty(AcceptHeader) ? other.AcceptHeader : AcceptHeader;
        ContentType = string.IsNullOrEmpty(ContentType) ? other.ContentType : ContentType;
        Content = string.IsNullOrEmpty(Content) ? other.Content : Content;
    }
}

namespace AdminApi.Tests.Integration.Utils.Models;

public class RequestConfiguration
{
    public AuthenticationParameters AuthenticationParameters { get; set; } = new AuthenticationParameters();
    public bool Authenticate { get; set; } = false;
    public string? ContentType { get; set; }
    public string? AcceptHeader { get; set; }
    public string? Content { get; set; }

    public void SupplementWith(RequestConfiguration other)
    {
        AuthenticationParameters = other.AuthenticationParameters;
        Authenticate = other.Authenticate;
        AcceptHeader ??= other.AcceptHeader;
        ContentType ??= other.ContentType;
        Content ??= other.Content;
    }
}

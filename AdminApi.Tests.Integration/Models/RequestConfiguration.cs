namespace AdminApi.Tests.Integration.Models;

public class RequestConfiguration
{
    public bool Authenticate { get; set; } = false;
    public string? ContentType { get; set; }
    public string? AcceptHeader { get; set; }
    public string? Content { get; set; }

    public void SupplementWith(RequestConfiguration other)
    {
        Authenticate = other.Authenticate;
        AcceptHeader ??= other.AcceptHeader;
        ContentType ??= other.ContentType;
        Content ??= other.Content;
    }
}

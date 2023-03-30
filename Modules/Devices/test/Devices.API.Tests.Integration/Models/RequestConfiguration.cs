namespace Devices.API.Tests.Integration.Models;
public class RequestConfiguration
{
    public string? ContentType { get; set; }
    public string? AcceptHeader { get; set; }
    public string? Content { get; set; }

    public void SupplementWith(RequestConfiguration other)
    {
        AcceptHeader ??= other.AcceptHeader;
        ContentType ??= other.ContentType;
        Content ??= other.Content;
    }
}

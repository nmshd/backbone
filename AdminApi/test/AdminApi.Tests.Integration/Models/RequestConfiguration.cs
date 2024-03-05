using Newtonsoft.Json;

namespace Backbone.AdminApi.Tests.Integration.Models;

public class RequestConfiguration
{
    public bool Authenticate { get; set; }
    public string? ContentType { get; set; }
    public string? AcceptHeader { get; set; }
    public string? Content { get; set; }

    public void SetContent(object obj)
    {
        Content = JsonConvert.SerializeObject(obj);
    }

    public void SupplementWith(RequestConfiguration other)
    {
        Authenticate = other.Authenticate;
        AcceptHeader ??= other.AcceptHeader;
        ContentType ??= other.ContentType;
        Content ??= other.Content;
    }

    public RequestConfiguration Clone()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(this);
        return requestConfiguration;
    }
}

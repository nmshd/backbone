using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class RequestConfiguration
{
    public AuthenticationParameters AuthenticationParameters { get; set; } = new();
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

    public RequestConfiguration Clone()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(this);
        return requestConfiguration;
    }

    public void SetContent(object obj)
    {
        Content = JsonConvert.SerializeObject(obj);
    }
}

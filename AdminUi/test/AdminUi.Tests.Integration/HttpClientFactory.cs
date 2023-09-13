using Enmeshed.Tooling.Extensions;

namespace AdminUi.Tests.Integration;

public class HttpClientFactory
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public HttpClientFactory(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public HttpClient CreateClient()
    {
        var baseAddress = Environment.GetEnvironmentVariable("ADMIN_API_BASE_ADDRESS");
        return baseAddress.IsNullOrEmpty() ? _factory.CreateClient() : new HttpClient() { BaseAddress = new Uri(baseAddress!) };
    }
}


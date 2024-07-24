using Backbone.Tooling.Extensions;

namespace Backbone.ConsumerApi.Tests.Integration;

internal class HttpClientFactory
{
    private readonly CustomWebApplicationFactory _factory;

    internal HttpClientFactory(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    internal HttpClient CreateClient()
    {
        var baseAddress = Environment.GetEnvironmentVariable("CONSUMER_API_BASE_ADDRESS");
        return baseAddress.IsNullOrEmpty() ? _factory.CreateClient() : new HttpClient() { BaseAddress = new Uri(baseAddress) };
    }
}

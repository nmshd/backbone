using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;

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
        return baseAddress.IsNullOrEmpty()
            ? _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false })
            : new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }) { BaseAddress = new Uri(baseAddress) };
    }
}

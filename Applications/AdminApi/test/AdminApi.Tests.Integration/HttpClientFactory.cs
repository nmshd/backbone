using Backbone.Tooling.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Backbone.AdminApi.Tests.Integration;

internal class HttpClientFactory
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    internal HttpClientFactory(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    internal HttpClient CreateClient()
    {
        var baseAddress = Environment.GetEnvironmentVariable("ADMIN_API_BASE_ADDRESS");
        return baseAddress.IsNullOrEmpty()
            ? _factory.CreateClient()
            : new HttpClient { BaseAddress = new Uri(baseAddress) };
    }
}

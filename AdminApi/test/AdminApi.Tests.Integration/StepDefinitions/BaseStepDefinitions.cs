using Backbone.AdminApi.Sdk;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

internal class BaseStepDefinitions
{
    protected readonly HttpClientOptions _options;
    protected readonly Client _client;

    public BaseStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options)
    {
        _options = options.Value;
        _client = Client.Create(factory.CreateClient(), _options.ApiKey);
    }
}

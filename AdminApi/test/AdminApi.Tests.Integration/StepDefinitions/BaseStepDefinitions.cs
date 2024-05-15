using Backbone.AdminApi.Sdk;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

internal abstract class BaseStepDefinitions
{
    protected readonly Client _client;

    protected BaseStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options)
    {
        _client = Client.Create(factory.CreateClient(), options.Value.ApiKey);
    }
}

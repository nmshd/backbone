using Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation;
using Backbone.AdminApi.Sdk.Endpoints.Clients;
using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Identities;
using Backbone.AdminApi.Sdk.Endpoints.Logs;
using Backbone.AdminApi.Sdk.Endpoints.Metrics;

namespace Backbone.AdminApi.Sdk;

public class Client
{
    public Client(Configuration config)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(config.BaseUrl) };
        var endpointClient = new EndpointClient(httpClient, config.ApiKey, config.JsonSerializerOptions);
    }
}

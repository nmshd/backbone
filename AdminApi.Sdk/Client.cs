using Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation;
using Backbone.AdminApi.Sdk.Endpoints.Challenges;
using Backbone.AdminApi.Sdk.Endpoints.Clients;
using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Identities;
using Backbone.AdminApi.Sdk.Endpoints.Logs;
using Backbone.AdminApi.Sdk.Endpoints.Metrics;
using Backbone.AdminApi.Sdk.Endpoints.Relationships;
using Backbone.AdminApi.Sdk.Endpoints.Tiers;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;

namespace Backbone.AdminApi.Sdk;

public class Client
{
    public Client(Configuration configuration) : this(new HttpClient { BaseAddress = new Uri(configuration.BaseUrl) }, configuration)
    {
    }

    public Client(HttpClient httpClient, Configuration config)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(config.BaseUrl) };
        var oDataClient = new HttpClient { BaseAddress = new Uri(config.ODataUrl) };
        var authenticator = new XsrfAndApiKeyAuthenticator(config.ApiKey, httpClient);
        var endpointClient = new EndpointClient(httpClient, authenticator, config.JsonSerializerOptions, oDataClient);

        ApiKeyValidation = new ApiKeyValidationEndpoint(endpointClient);
        Clients = new ClientsEndpoint(endpointClient);
        Identities = new IdentitiesEndpoint(endpointClient);
        Logs = new LogsEndpoint(endpointClient);
        Metrics = new MetricsEndpoint(endpointClient);
        Relationships = new RelationshipsEndpoint(endpointClient);
        Tiers = new TiersEndpoint(endpointClient);
        Challenges = new ChallengesEndpoint(endpointClient);
    }

    public ApiKeyValidationEndpoint ApiKeyValidation { get; }
    public ClientsEndpoint Clients { get; }
    public IdentitiesEndpoint Identities { get; }
    public LogsEndpoint Logs { get; }
    public MetricsEndpoint Metrics { get; }
    public RelationshipsEndpoint Relationships { get; }
    public TiersEndpoint Tiers { get; }
    public ChallengesEndpoint Challenges { get; }
}

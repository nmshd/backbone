using System.Text.Json;
using Backbone.AdminApi.Sdk.Authentication;
using Backbone.AdminApi.Sdk.Endpoints.Announcements;
using Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation;
using Backbone.AdminApi.Sdk.Endpoints.Challenges;
using Backbone.AdminApi.Sdk.Endpoints.Clients;
using Backbone.AdminApi.Sdk.Endpoints.Identities;
using Backbone.AdminApi.Sdk.Endpoints.Messages;
using Backbone.AdminApi.Sdk.Endpoints.Metrics;
using Backbone.AdminApi.Sdk.Endpoints.Relationships;
using Backbone.AdminApi.Sdk.Endpoints.Tiers;
using Backbone.AdminApi.Sdk.Endpoints.Tokens;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.Tooling.JsonConverters;

namespace Backbone.AdminApi.Sdk;

public class Client
{
    private Client(HttpClient httpClient, string apiKey)
    {
        var authenticator = new XsrfAndApiKeyAuthenticator(apiKey, httpClient);

        var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        jsonSerializerOptions.Converters.Add(new UrlSafeBase64ToByteArrayJsonConverter());

        var endpointClient = new EndpointClient(httpClient, authenticator, jsonSerializerOptions);

        ApiKeyValidation = new ApiKeyValidationEndpoint(endpointClient);
        Announcements = new AnnouncementsEndpoint(endpointClient);
        Clients = new ClientsEndpoint(endpointClient);
        Identities = new IdentitiesEndpoint(endpointClient);
        Metrics = new MetricsEndpoint(endpointClient);
        Relationships = new RelationshipsEndpoint(endpointClient);
        Tiers = new TiersEndpoint(endpointClient);
        Challenges = new ChallengesEndpoint(endpointClient);
        Messages = new MessagesEndpoint(endpointClient);
        Tokens = new TokensEndpoint(endpointClient);
    }

    public ApiKeyValidationEndpoint ApiKeyValidation { get; }
    public AnnouncementsEndpoint Announcements { get; }
    public ClientsEndpoint Clients { get; }
    public IdentitiesEndpoint Identities { get; }
    public MetricsEndpoint Metrics { get; }
    public RelationshipsEndpoint Relationships { get; }
    public TiersEndpoint Tiers { get; }
    public ChallengesEndpoint Challenges { get; }
    public MessagesEndpoint Messages { get; }

    public TokensEndpoint Tokens { get; }

    public static Client Create(string baseUrl, string apiKey)
    {
        return Create(new HttpClient { BaseAddress = new Uri(baseUrl) }, apiKey);
    }

    public static Client Create(HttpClient httpClient, string apiKey)
    {
        return new Client(httpClient, apiKey);
    }
}

﻿using System.Text.Json;
using Backbone.AdminApi.Sdk.Authentication;
using Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation;
using Backbone.AdminApi.Sdk.Endpoints.Challenges;
using Backbone.AdminApi.Sdk.Endpoints.Clients;
using Backbone.AdminApi.Sdk.Endpoints.Identities;
using Backbone.AdminApi.Sdk.Endpoints.Logs;
using Backbone.AdminApi.Sdk.Endpoints.Metrics;
using Backbone.AdminApi.Sdk.Endpoints.Relationships;
using Backbone.AdminApi.Sdk.Endpoints.Tiers;
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

    public static Client Create(string baseUrl, string apiKey)
    {
        return Create(new HttpClient { BaseAddress = new Uri(baseUrl) }, apiKey);
    }

    public static Client Create(HttpClient httpClient, string apiKey)
    {
        return new Client(httpClient, apiKey);
    }
}

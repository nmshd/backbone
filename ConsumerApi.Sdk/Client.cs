using Backbone.ConsumerApi.Sdk.Endpoints.Challenges;
using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices;
using Backbone.ConsumerApi.Sdk.Endpoints.Files;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications;
using Backbone.ConsumerApi.Sdk.Endpoints.Quotas;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens;

namespace Backbone.ConsumerApi.Sdk;

public class Client
{
    public Client(Configuration configuration)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(configuration.BaseUrl) };
        var authenticator = new Authenticator(configuration.Authentication, httpClient);
        var endpointClient = new EndpointClient(httpClient, authenticator, configuration.JsonSerializerOptions);

        Challenges = new ChallengesEndpoint(endpointClient);
        Datawallet = new DatawalletEndpoint(endpointClient);
        Devices = new DevicesEndpoint(endpointClient);
        Files = new FilesEndpoint(endpointClient);
        Identities = new IdentitiesEndpoint(endpointClient);
        Messages = new MessagesEndpoint(endpointClient);
        PushNotifications = new PushNotificationsEndpoint(endpointClient);
        Quotas = new QuotasEndpoint(endpointClient);
        Relationships = new RelationshipsEndpoint(endpointClient);
        RelationshipTemplates = new RelationshipTemplatesEndpoint(endpointClient);
        SyncRuns = new SyncRunsEndpoint(endpointClient);
        Tokens = new TokensEndpoint(endpointClient);
    }

    public ChallengesEndpoint Challenges { get; }
    public DatawalletEndpoint Datawallet { get; }
    public DevicesEndpoint Devices { get; }
    public FilesEndpoint Files { get; }
    public IdentitiesEndpoint Identities { get; }
    public MessagesEndpoint Messages { get; }
    public PushNotificationsEndpoint PushNotifications { get; }
    public QuotasEndpoint Quotas { get; }
    public RelationshipsEndpoint Relationships { get; }
    public RelationshipTemplatesEndpoint RelationshipTemplates { get; }
    public SyncRunsEndpoint SyncRuns { get; }
    public TokensEndpoint Tokens { get; }
}

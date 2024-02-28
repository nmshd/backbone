using Backbone.ConsumerApi.Sdk.Endpoints.Challenges;
using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices;
using Backbone.ConsumerApi.Sdk.Endpoints.Files;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications;
using Backbone.ConsumerApi.Sdk.Endpoints.Quotas;

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
    }

    public ChallengesEndpoint Challenges { get; }
    public DatawalletEndpoint Datawallet { get; }
    public DevicesEndpoint Devices { get; }
    public FilesEndpoint Files { get; }
    public IdentitiesEndpoint Identities { get; }
    public MessagesEndpoint Messages { get; }
    public PushNotificationsEndpoint PushNotifications { get; }
    public QuotasEndpoint Quotas { get; }
}

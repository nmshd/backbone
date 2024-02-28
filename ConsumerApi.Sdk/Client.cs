using Backbone.ConsumerApi.Sdk.Endpoints.Challenges;
using Backbone.ConsumerApi.Sdk.Endpoints.Common;

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
    }

    public ChallengesEndpoint Challenges { get; }
    public DatawalletEndpoint Datawallet { get; }
    public DevicesEndpoint Devices { get; }
    public FilesEndpoint Files { get; }
    public IdentitiesEndpoint Identities { get; }
}

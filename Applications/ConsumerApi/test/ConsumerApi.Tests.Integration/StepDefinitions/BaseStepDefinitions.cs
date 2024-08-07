using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

internal class BaseStepDefinitions
{
    internal readonly Dictionary<string, Client> Identities = new();
    internal readonly HttpClient HttpClient;
    internal readonly ClientCredentials ClientCredentials;


    public BaseStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        HttpClient = factory.CreateClient();
        ClientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

    #region Given


    [Given(@"Identities (i[a-zA-Z0-9]*) and (i[a-zA-Z0-9]*)")]
    public void Given2Identities(string identity1Name, string identity2Name)
    {
        Identities[identity1Name] = Client.CreateForNewIdentity(HttpClient, ClientCredentials, Constants.DEVICE_PASSWORD).Result;
        Identities[identity2Name] = Client.CreateForNewIdentity(HttpClient, ClientCredentials, Constants.DEVICE_PASSWORD).Result;
    }

    [Given(@"Identities (i[a-zA-Z0-9]*), (i[a-zA-Z0-9]*) and (i[a-zA-Z0-9]*)")]
    public void Given3Identities(string identity1Name, string identity2Name, string identity3Name)
    {
        Identities[identity1Name] = Client.CreateForNewIdentity(HttpClient, ClientCredentials, Constants.DEVICE_PASSWORD).Result;
        Identities[identity2Name] = Client.CreateForNewIdentity(HttpClient, ClientCredentials, Constants.DEVICE_PASSWORD).Result;
        Identities[identity3Name] = Client.CreateForNewIdentity(HttpClient, ClientCredentials, Constants.DEVICE_PASSWORD).Result;
    }
    
    #endregion

}

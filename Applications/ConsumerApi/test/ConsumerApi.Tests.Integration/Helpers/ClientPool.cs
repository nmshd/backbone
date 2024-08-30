using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.Helpers;

public class ClientPool
{
    private const string DEFAULT_DEVICE_NAME = "";

    private readonly List<ClientWrapper> _clientWrappers = [];

    internal ClientPool(HttpClientFactory httpClientFactory, IOptions<HttpConfiguration> configuration)
    {
        var clientCredentials = new ClientCredentials(configuration.Value.ClientCredentials.ClientId, configuration.Value.ClientCredentials.ClientSecret);
        Anonymous = Client.CreateUnauthenticated(httpClientFactory.CreateClient(), clientCredentials);
    }

    public Client Anonymous { get; private set; }

    public ClientAdder Add(Client client)
    {
        return new ClientAdder(this, client);
    }

    public Client FirstForIdentityName(string identityName) => _clientWrappers.First(c => c.IdentityName == identityName).Client;
    public Client FirstForIdentityAddress(string identityAddress) => _clientWrappers.First(c => c.Client.IdentityData?.Address == identityAddress).Client;

    public Client[] GetAllForIdentityNames(List<string> identityNames) =>
        _clientWrappers.Where(cw => cw.IdentityName != null && identityNames.Contains(cw.IdentityName)).Select(cw => cw.Client).ToArray();

    public Client GetForDeviceName(string deviceName) => _clientWrappers.First(c => c.DeviceName == deviceName).Client;

    public string? GetIdentityNameForDevice(string deviceName) => _clientWrappers.FirstOrDefault(cw => cw.DeviceName == deviceName)!.IdentityName;

    private class ClientWrapper
    {
        public required Client Client { get; set; } // todo: tidy this up
        public string? IdentityName { get; set; }
        public string? DeviceName { get; set; }
    }

    public class ClientAdder
    {
        private readonly ClientWrapper _clientWrapper;

        public ClientAdder(ClientPool manager, Client client)
        {
            _clientWrapper = new ClientWrapper { Client = client };
            manager._clientWrappers.Add(_clientWrapper);
        }

        public ClientAdder ForIdentity(string identity)
        {
            _clientWrapper.IdentityName = identity;
            _clientWrapper.DeviceName = DEFAULT_DEVICE_NAME;
            return this;
        }

        public void AndDevice(string device)
        {
            _clientWrapper.DeviceName = device;
        }
    }
}

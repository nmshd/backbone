using Backbone.ConsumerApi.Sdk;

namespace Backbone.ConsumerApi.Tests.Integration.Helpers;

public class ClientPool
{
    private const string DEFAULT_IDENTITY_NAME = "";
    private const string DEFAULT_DEVICE_NAME = "";

    private readonly List<ClientWrapper> _clientWrappers = [];

    public void AddAnonymous(Client client)
    {
        Anonymous = client;
    }

    public Client? Anonymous { get; private set; }

    public ClientAdder Add(Client client)
    {
        return new ClientAdder(this, client);
    }

    public Client? Default()
    {
        if (!IsOnlyOneClientInThePool)
            throw new InvalidOperationException("No identity is considered 'default identity' when there is more than one in the pool. Use the required identity's key to access it instead.");

        return FirstForDefaultIdentity() ?? Anonymous;
    }

    public bool IsDefaultClientAuthenticated()
    {
        if (!IsOnlyOneClientInThePool)
            throw new InvalidOperationException("No identity is considered 'default identity' when there is more than one in the pool. Use the required identity's key to access it instead.");

        return FirstForDefaultIdentity() != null && Anonymous == null;
    }

    private bool IsOnlyOneClientInThePool => Anonymous != null && _clientWrappers.Count == 0 || Anonymous == null && _clientWrappers.Select(cw => cw.IdentityName).Distinct().Count() == 1;

    public Client? FirstForDefaultIdentity() => _clientWrappers.FirstOrDefault()?.Client;
    public Client FirstForIdentityName(string identityName) => _clientWrappers.First(c => c.IdentityName == identityName).Client;
    public Client FirstForIdentityAddress(string identityAddress) => _clientWrappers.First(c => c.Client.IdentityData?.Address == identityAddress).Client;

    public Client[] GetClientsByIdentities(List<string> identityNames) =>
        _clientWrappers.Where(cw => cw.IdentityName != null && identityNames.Contains(cw.IdentityName)).Select(cw => cw.Client).ToArray();

    public Client GetForDeviceName(string deviceName) => _clientWrappers.First(c => c.DeviceName == deviceName).Client;

    public string? GetIdentityForDevice(string deviceName) => _clientWrappers.FirstOrDefault(cw => cw.DeviceName == deviceName)!.IdentityName;

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

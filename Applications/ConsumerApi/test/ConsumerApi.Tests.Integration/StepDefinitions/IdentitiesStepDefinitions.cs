using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.Utils;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class IdentitiesStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;

    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public IdentitiesStepDefinitions(IdentitiesContext identitiesContext, ResponseContext responseContext, HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private ClientPool ClientPool => _identitiesContext.ClientPool;

    #endregion

    #region Given

    [Given(@"Identity ([a-zA-Z0-9]+)")]
    public async Task GivenIdentity(string identityName)
    {
        await CreateClientForIdentityName(identityName);
    }

    [Given(@"Identities ([a-zA-Z0-9, ]+)")]
    public async Task GivenIdentities(string identityNames)
    {
        foreach (var identityName in SplitNames(identityNames))
            await CreateClientForIdentityName(identityName);
    }

    [Given("the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        var client = Client.CreateUnauthenticated(_httpClient, _clientCredentials);
        ClientPool.AddAnonymous(client);
    }

    [Given("Identities ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+) with an established Relationship")]
    public async Task GivenIdentitiesWithAnEstablishedRelationship(string identity1Name, string identity2Name)
    {
        await CreateClientForIdentityName(identity1Name);
        await CreateClientForIdentityName(identity2Name);

        await EstablishRelationshipBetween(ClientPool.FirstForIdentityName(identity1Name), ClientPool.FirstForIdentityName(identity2Name));
    }

    #endregion

    #region When

    [When("a POST request is sent to the /Identities endpoint with a valid signature on c")]
    public async Task WhenAPostRequestIsSentToTheIdentitiesEndpointWithAValidSignatureOnChallenge()
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();
        var identityKeyPair = signatureHelper.CreateKeyPair();

        var serializedChallenge = JsonConvert.SerializeObject(_responseContext.ChallengeResponse!.Result);
        var challengeSignature = signatureHelper.CreateSignature(identityKeyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));
        var signedChallenge = new SignedChallenge(serializedChallenge, challengeSignature);

        var createIdentityPayload = new CreateIdentityRequest
        {
            ClientId = _clientCredentials.ClientId,
            ClientSecret = _clientCredentials.ClientSecret,
            IdentityVersion = 1,
            SignedChallenge = signedChallenge,
            IdentityPublicKey = ConvertibleString.FromUtf8(JsonConvert.SerializeObject(new CryptoSignaturePublicKey
            {
                alg = CryptoExchangeAlgorithm.ECDH_X25519,
                pub = identityKeyPair.PublicKey.Base64Representation
            })).BytesRepresentation,
            DevicePassword = DEVICE_PASSWORD
        };

        var client = ClientPool.Default();
        _responseContext.WhenResponse = _responseContext.CreateIdentityResponse = await client!.Identities.CreateIdentity(createIdentityPayload);
    }

    #endregion

    private async Task CreateClientForIdentityName(string identityName)
    {
        var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);
        _identitiesContext.ClientPool.Add(client).ForIdentity(identityName);
    }
}

public class IdentitiesContext
{
    public ClientPool ClientPool { get; } = new();
    public readonly Dictionary<string, string> ActiveDeletionProcesses = new();
}

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

using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
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

    private readonly ResponseContext _responseContext;
    private readonly ChallengesContext _challengesContext;
    private readonly ClientPool _clientPool;

    private ApiResponse<IsDeletedResponse>? _isDeletedResponse;

    public IdentitiesStepDefinitions(ResponseContext responseContext, ChallengesContext challengesContext, ClientPool clientPool, HttpClientFactory factory,
        IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _responseContext = responseContext;
        _challengesContext = challengesContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($"Identity {RegexFor.SINGLE_THING}")]
    public async Task GivenIdentity(string identityName)
    {
        var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);
        _clientPool.Add(client).ForIdentity(identityName);
    }

    [Given($"Identities {RegexFor.LIST_OF_THINGS}")]
    public async Task GivenIdentities(string identityNames)
    {
        foreach (var identityName in SplitNames(identityNames))
        {
            var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);
            _clientPool.Add(client).ForIdentity(identityName);
        }
    }

    #endregion

    #region When

    [When($"an anonymous user sends a POST request to the /Identities endpoint with a valid signature on {RegexFor.SINGLE_THING}")]
    public async Task WhenAPostRequestIsSentToTheIdentitiesEndpointWithAValidSignatureOnChallenge(string challengeName)
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();
        var identityKeyPair = signatureHelper.CreateKeyPair();

        var serializedChallenge = JsonConvert.SerializeObject(_challengesContext.Challenges[challengeName]);
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
            CommunicationLanguage = "en",
            DevicePassword = DEVICE_PASSWORD
        };

        _responseContext.WhenResponse = await _clientPool.Anonymous.Identities.CreateIdentity(createIdentityPayload);
    }

    #endregion

    [When($@"an anonymous user sends a GET request to the /Identities/IsDeleted endpoint with {RegexFor.SINGLE_THING}.Username")]
    public async Task WhenAnAnonymousUserSendsAGETRequestToTheIdentitiesIsDeletedEndpointWithDUsername(string deviceName)
    {
        var client = _clientPool.GetForDeviceName(deviceName);

        var device = await client.Devices.GetActiveDevice();

        _responseContext.WhenResponse = _isDeletedResponse = await _clientPool.Anonymous.Identities.IsDeleted(device.Result!.Username);
    }

    [Then(@"the response says that the identity was not deleted")]
    public void ThenTheResponseSaysThatTheIdentityWasNotDeleted()
    {
        _isDeletedResponse!.Result!.IsDeleted.Should().BeFalse();
    }

    [Then(@"the deletion date is not set")]
    public void ThenTheDeletionDateIsNotSet()
    {
        _isDeletedResponse!.Result!.DeletionDate.Should().BeNull();
    }
}

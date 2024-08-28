using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
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
    private readonly ClientPool _clientPool;

    public IdentitiesStepDefinitions(ResponseContext responseContext, ClientPool clientPool, HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($"Identity {RegexFor.SINGLE_THING}")]
    public async Task GivenIdentity(string identityName)
    {
        await CreateClientForIdentityName(identityName);
    }

    [Given($"Identities {RegexFor.LIST_OF_THINGS}")]
    public async Task GivenIdentities(string identityNames)
    {
        foreach (var identityName in SplitNames(identityNames))
            await CreateClientForIdentityName(identityName);
    }

    [Given("the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        var client = Client.CreateUnauthenticated(_httpClient, _clientCredentials);
        _clientPool.AddAnonymous(client);
    }

    [Given($"Identities {RegexFor.SINGLE_THING}{RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING}{RegexFor.SINGLE_THING} with an established Relationship")]
    public async Task GivenIdentitiesWithAnEstablishedRelationship(string identity1Name, string identity2Name)
    {
        await CreateClientForIdentityName(identity1Name);
        await CreateClientForIdentityName(identity2Name);

        await EstablishRelationshipBetween(_clientPool.FirstForIdentityName(identity1Name), _clientPool.FirstForIdentityName(identity2Name));
    }

    #endregion

    #region When

    [When("an anonymous user sends a POST request to the /Identities endpoint with a valid signature on c")]
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

        _responseContext.WhenResponse = _responseContext.CreateIdentityResponse = await _clientPool.Anonymous.Identities.CreateIdentity(createIdentityPayload);
    }

    #endregion

    private async Task CreateClientForIdentityName(string identityName)
    {
        var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);
        _clientPool.Add(client).ForIdentity(identityName);
    }
}

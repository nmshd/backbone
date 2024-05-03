using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.SdkStepDefinitions;

internal class IdentitiesApiStepDefinitions
{
    private Client? _client;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;
    private ApiResponse<StartDeletionProcessResponse>? _response;
    private ApiResponse<CreateIdentityResponse>? _identityResponse;
    private ApiResponse<Challenge>? _challengeResponse;
    private IdentityData? _identityData;

    public IdentitiesApiStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

    [Given("no active deletion process for the identity exists")]
    public void GivenNoActiveDeletionProcessForTheUserExists()
    {
    }

    [Given("an active deletion process for the identity exists")]
    public async Task GivenAnActiveDeletionProcessForTheUserExists()
    {
        await _client!.Identities.StartDeletionProcess();
    }

    [When("a POST request is sent to the /Identities/Self/DeletionProcesses endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesSelfDeletionProcessEndpoint()
    {
        _response = await _client!.Identities.StartDeletionProcess();
    }

    [Then(@"the response content includes an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _response!.Error.Should().NotBeNull();
        _response.Error!.Code.Should().Be(errorCode);
    }

    [Then("the response contains a Deletion Process")]
    public void ThenTheResponseContainsADeletionProcess()
    {
        _response!.Result.Should().NotBeNull();
        // _response!.AssertContentCompliesWithSchema();
    }

    [Given("a Challenge c")]
    public async Task GivenAChallengeC()
    {
        _client = Client.CreateUnauthenticated(_httpClient, _clientCredentials);
        _challengeResponse = await _client.Challenges.CreateChallengeUnauthenticated();
    }

    [When("a POST request is sent to the /Identities endpoint with a valid signature on c")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesEndpoint()
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();
        var identityKeyPair = signatureHelper.CreateKeyPair();

        var serializedChallenge = JsonConvert.SerializeObject(_challengeResponse!.Result);

        var challengeSignature = signatureHelper.CreateSignature(identityKeyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));
        var signedChallenge = new SignedChallenge { Challenge = serializedChallenge, Signature = challengeSignature.BytesRepresentation };

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
            DevicePassword = "someDevicePassword"
        };

        _identityResponse = await _client!.Identities.CreateIdentity(createIdentityPayload);
    }

    [Given("an Identity i")]
    public async Task GivenAnIdentityI()
    {
        _client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, "somePassword");
        _identityData = _client.IdentityData;
    }

    [Then("the response contains a CreateIdentityResponse")]
    public void ThenTheResponseContainsACreateIdentityResponse()
    {
        _identityResponse!.Should().NotBeNull();
        _identityResponse!.IsSuccess.Should().BeTrue();
        // _identityResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_identityResponse != null)
        {
            ((int)_identityResponse!.Status).Should().Be(expectedStatusCode);
        }

        if (_response != null)
        {
            ((int)_response!.Status).Should().Be(expectedStatusCode);
        }
    }
}

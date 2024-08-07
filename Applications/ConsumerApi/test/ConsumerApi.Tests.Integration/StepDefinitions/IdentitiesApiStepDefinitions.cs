using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Identities/Self/DeletionProcess")]
[Scope(Feature = "PUT Identities/Self/DeletionProcesses/{id}/Approve")]
[Scope(Feature = "POST Identity")]
internal class IdentitiesApiStepDefinitions
{
    private Client _sdk = null!;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;
    private ApiResponse<StartDeletionProcessResponse>? _startDeletionProcessResponse;
    private ApiResponse<ApproveDeletionProcessResponse>? _approveDeletionProcessResponse;
    private ApiResponse<CreateIdentityResponse>? _identityResponse;
    private ApiResponse<Challenge>? _challengeResponse;

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
        await _sdk.Identities.StartDeletionProcess();
    }

    [Given("an Identity i")]
    public async Task GivenAnIdentityI()
    {
        _sdk = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
    }

    [Given("a Challenge c")]
    public async Task GivenAChallengeC()
    {
        _sdk = Client.CreateUnauthenticated(_httpClient, _clientCredentials);
        _challengeResponse = await _sdk.Challenges.CreateChallengeUnauthenticated();
    }

    [When("a POST request is sent to the /Identities/Self/DeletionProcesses endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesSelfDeletionProcessEndpoint()
    {
        _startDeletionProcessResponse = await _sdk.Identities.StartDeletionProcess();
    }

    [When("a PUT request is sent to the /Identities/Self/DeletionProcesses/{d.id}/Approve endpoint with a non-existent deletionProcessId")]
    public async Task WhenAPutRequestIsSentToTheIdentitiesSelfDeletionProcessesIdApproveEndpointWithANonExistentDeletionProcessId()
    {
        _approveDeletionProcessResponse = await _sdk.Identities.ApproveDeletionProcess("IDPSomeNonExistentId");
    }

    [When("a POST request is sent to the /Identities endpoint with a valid signature on c")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesEndpoint()
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();
        var identityKeyPair = signatureHelper.CreateKeyPair();

        var serializedChallenge = JsonConvert.SerializeObject(_challengeResponse!.Result);
        var challengeSignature = signatureHelper.CreateSignature(identityKeyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));
        var signedChallenge = new SignedChallenge(serializedChallenge, challengeSignature);

        var createIdentityPayload = new CreateIdentityRequest
        {
            ClientId = "test",
            ClientSecret = "test",
            IdentityVersion = 1,
            SignedChallenge = signedChallenge,
            IdentityPublicKey = ConvertibleString.FromUtf8(JsonConvert.SerializeObject(new CryptoSignaturePublicKey
            {
                alg = CryptoExchangeAlgorithm.ECDH_X25519,
                pub = identityKeyPair.PublicKey.Base64Representation
            })).BytesRepresentation,
            DevicePassword = "some-device-password"
        };

        _identityResponse = await _sdk.Identities.CreateIdentity(createIdentityPayload);
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_startDeletionProcessResponse != null)
        {
            _startDeletionProcessResponse!.Error.Should().NotBeNull();
            _startDeletionProcessResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_approveDeletionProcessResponse != null)
        {
            _approveDeletionProcessResponse!.Error.Should().NotBeNull();
            _approveDeletionProcessResponse.Error!.Code.Should().Be(errorCode);
        }
    }

    [Then("the response contains a Deletion Process")]
    public async Task ThenTheResponseContainsADeletionProcess()
    {
        _startDeletionProcessResponse!.Result.Should().NotBeNull();
        _startDeletionProcessResponse.Should().BeASuccess();
        await _startDeletionProcessResponse.Should().ComplyWithSchema();
    }

    [Then("the response contains a CreateIdentityResponse")]
    public async Task ThenTheResponseContainsACreateIdentityResponse()
    {
        _identityResponse!.Should().NotBeNull();
        _identityResponse!.Should().BeASuccess();
        await _identityResponse!.Should().ComplyWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_identityResponse != null)
            ((int)_identityResponse!.Status).Should().Be(expectedStatusCode);

        if (_startDeletionProcessResponse != null)
            ((int)_startDeletionProcessResponse!.Status).Should().Be(expectedStatusCode);
    }
}

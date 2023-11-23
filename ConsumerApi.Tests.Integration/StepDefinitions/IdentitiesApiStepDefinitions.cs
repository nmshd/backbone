using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto;
using Backbone.Crypto.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Identity")]
internal class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private readonly ChallengesApi _challengeApi;
    private readonly ISignatureHelper _signatureHelper;
    private readonly IdentitiesApi _identitiesApi;
    private HttpResponse<CreateIdentityResponse>? _identityResponse;
    private HttpResponse<Challenge>? _challengeResponse;

    public IdentitiesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, IdentitiesApi identitiesApi, ChallengesApi challengeApi, ISignatureHelper signatureHelper) : base(httpConfiguration)
    {
        _identitiesApi = identitiesApi;
        _challengeApi = challengeApi;
        _signatureHelper = signatureHelper;
    }

    [Given(@"a Challenge c")]
    public async Task GivenAChallengeC()
    {
        await CreateChallenge();
    }

    [When(@"a POST request is sent to the /Identities endpoint with a valid signature on c")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesEndpoint()
    {
        await CreateIdentity();
    }

    [Given(@"an Identity i")]
    public async Task GivenAnIdentityI()
    {
        await CreateChallenge();
        await CreateIdentity();
    }

    [Then(@"the response contains a CreateIdentityResponse")]
    public void ThenTheResponseContainsACreateIdentityResponse()
    {
        _identityResponse!.Should().NotBeNull();
        _identityResponse!.IsSuccessStatusCode.Should().BeTrue();
        _identityResponse!.ContentType.Should().Be("application/json");
        _identityResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_identityResponse!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    private async Task CreateChallenge()
    {
        _challengeResponse = await _challengeApi.CreateChallenge(_requestConfiguration);
        _challengeResponse!.IsSuccessStatusCode.Should().BeTrue();
    }

    private async Task CreateIdentity()
    {
        var serializedChallenge = JsonConvert.SerializeObject(_challengeResponse!.Content.Result!);

        var keyPair = _signatureHelper.CreateKeyPair();
        var signature = _signatureHelper.CreateSignature(keyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));

        dynamic publicKey = new
        {
            pub = keyPair.PublicKey.Base64Representation,
            alg = 3
        };

        dynamic signedChallenge = new
        {
            sig = signature.BytesRepresentation,
            alg = 2
        };

        var createIdentityRequest = new CreateIdentityRequest()
        {
            ClientId = "test",
            ClientSecret = "test",
            DevicePassword = "test",
            IdentityPublicKey = (ConvertibleString.FromUtf8(JsonConvert.SerializeObject(publicKey)) as ConvertibleString)!.Base64Representation,
            IdentityVersion = 1,
            SignedChallenge = new CreateIdentityRequestSignedChallenge()
            {
                Challenge = serializedChallenge,
                Signature = (ConvertibleString.FromUtf8(JsonConvert.SerializeObject(signedChallenge)) as ConvertibleString)!.Base64Representation
            }
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createIdentityRequest);

        _identityResponse = await _identitiesApi.CreateIdentity(requestConfiguration);
    }
}

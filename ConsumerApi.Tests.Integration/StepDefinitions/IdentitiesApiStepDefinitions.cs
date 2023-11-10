using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Identity")]
internal class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private readonly ChallengesApi _challengeApi;
    private readonly IdentitiesApi _identitiesApi;
    private HttpResponse<CreateIdentityResponse>? _identityResponse;
    private HttpResponse<Challenge>? _challengeResponse;

    public IdentitiesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, IdentitiesApi identitiesApi, ChallengesApi challengeApi) : base(httpConfiguration)
    {
        _identitiesApi = identitiesApi;
        _challengeApi = challengeApi;
    }

    [Given(@"a Challenge c")]
    public async Task GivenAChallengeC()
    {
        _challengeResponse = await _challengeApi.CreateChallenge(_requestConfiguration);
        _challengeResponse!.IsSuccessStatusCode.Should().BeTrue();
    }

    [When(@"a POST request is sent to the /Identities endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesEndpoint()
    {
        var serializedChallenge = JsonConvert.SerializeObject(new
        {
            _challengeResponse!.Content.Result!.Id,
            _challengeResponse!.Content.Result!.ExpiresAt,
            Type = "Identity"
        });

        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();
        var keyPair = signatureHelper.CreateKeyPair();
        var signature = signatureHelper.CreateSignature(ConvertibleString.FromUtf8(serializedChallenge), keyPair.PrivateKey);

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

    [Then(@"the response contains a CreateIdentityResponse")]
    public void ThenTheResponseContainsACreateIdentityResponse()
    {
        _identityResponse!.AssertHasValue();
        _identityResponse!.AssertStatusCodeIsSuccess();
        _identityResponse!.AssertContentTypeIs("application/json");
        _identityResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_identityResponse!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }
}

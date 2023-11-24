using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto;
using Backbone.Crypto.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class BaseStepDefinitions
{
    protected readonly RequestConfiguration _requestConfiguration;
    private readonly ISignatureHelper _signatureHelper;
    private readonly ChallengesApi _challengesApi;
    private readonly IdentitiesApi _identitiesApi;

    public BaseStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, ISignatureHelper signatureHelper, ChallengesApi challengesApi, IdentitiesApi identitiesApi)
    {
        _requestConfiguration = new RequestConfiguration
        {
            AuthenticationParameters = new AuthenticationParameters
            {
                GrantType = "password",
                ClientId = httpConfiguration.Value.ClientCredentials.ClientId,
                ClientSecret = httpConfiguration.Value.ClientCredentials.ClientSecret,
                Username = "USRa",
                Password = "a"
            }
        };

        _signatureHelper = signatureHelper;
        _challengesApi = challengesApi;
        _identitiesApi = identitiesApi;
    }

    [Given(@"the user is authenticated")]
    public void GivenTheUserIsAuthenticated()
    {
        _requestConfiguration.Authenticate = true;
    }

    [Given(@"the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _requestConfiguration.Authenticate = false;
    }

    [Given(@"the Accept header is '([^']*)'")]
    public void GivenTheAcceptHeaderIs(string acceptHeader)
    {
        _requestConfiguration.AcceptHeader = acceptHeader;
    }

    protected async Task<HttpResponse<Challenge>> CreateChallenge()
    {
        var challengeResponse = await _challengesApi.CreateChallenge(_requestConfiguration);
        challengeResponse.IsSuccessStatusCode.Should().BeTrue();

        return challengeResponse;
    }

    protected async Task<HttpResponse<CreateIdentityResponse>> CreateIdentity(Challenge? challenge = null, KeyPair? keyPair = null)
    {
        challenge ??= (await CreateChallenge()).Content.Result!;
        keyPair ??= _signatureHelper.CreateKeyPair();

        var serializedChallenge = JsonConvert.SerializeObject(challenge);

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

        var identityResponse = await _identitiesApi.CreateIdentity(requestConfiguration);
        identityResponse.IsSuccessStatusCode.Should().BeTrue();

        return identityResponse;
    }

    protected void Authenticate(string username, string password)
    {
        _requestConfiguration.AuthenticationParameters.Username = username;
        _requestConfiguration.AuthenticationParameters.Password = password;
    }
}

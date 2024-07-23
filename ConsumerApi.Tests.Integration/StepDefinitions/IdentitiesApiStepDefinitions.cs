using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
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
using static Backbone.ConsumerApi.Tests.Integration.Helpers.Utils;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class IdentitiesApiStepDefinitions
{
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;

    private readonly ChallengesContext _challengesContext;
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public IdentitiesApiStepDefinitions(ChallengesContext challengesContext, IdentitiesContext identitiesContext, ResponseContext responseContext, HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _challengesContext = challengesContext;
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private Client AnonymousClient => _identitiesContext.AnonymousClient!;
    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];
    private ApiResponse<StartDeletionProcessResponse> StartDeletionProcessResponse => _responseContext.StartDeletionProcessResponse!;

    #region Given
    [Given(@"Identity (.+)")]
    public async Task GivenIdentity(string identityName)
    {
        _challengesContext.IsAuthenticated = true;
        await CreateAuthenticated(_identitiesContext, _httpClient, _clientCredentials, identityName);
    }

    [Given("the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _challengesContext.IsAuthenticated = false;
        CreateUnauthenticated(_identitiesContext, _httpClient, _clientCredentials);
    }

    [Given("no active deletion process for the identity exists")]
    public void GivenNoActiveDeletionProcessForTheUserExists() { }

    [Given("an active deletion process for (.+) exists")]
    public async Task GivenAnActiveDeletionProcessForTheIdentityExists(string identityName)
    {
        await Identity(identityName).Identities.StartDeletionProcess();
    }

    [Given("Identities (.+) and (.+) with an established Relationship")]
    public async Task GivenIdentitiesI1AndI2WithAnEstablishedRelationship(string identity1Name, string identity2Name)
    {
        await CreateAuthenticated(_identitiesContext, _httpClient, _clientCredentials, identity1Name);
        await CreateAuthenticated(_identitiesContext, _httpClient, _clientCredentials, identity2Name);

        await EstablishRelationshipBetween(Identity(identity1Name), Identity(identity2Name));
    }

    [Given("(.+) is in status \"ToBeDeleted\"")]
    public async Task GivenIdentityIsToBeDeleted(string identityName)
    {
        _responseContext.StartDeletionProcessResponse = await Identity(identityName).Identities.StartDeletionProcess();
        StartDeletionProcessResponse.Should().BeASuccess();
    }

    [Given(@"Identities (.+)")]
    public void GivenIdentities(string identityNames)
    {
        foreach (var identityName in SplitNames(identityNames))
            _identitiesContext.Identities[identityName] = Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD).Result;
    }
    #endregion

    #region When
    [When("a POST request is sent to the Identities endpoint with a valid signature on c")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesEndpoint()
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();
        var identityKeyPair = signatureHelper.CreateKeyPair();

        var serializedChallenge = JsonConvert.SerializeObject(_responseContext.ChallengeResponse!.Result);
        var challengeSignature = signatureHelper.CreateSignature(identityKeyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));
        var signedChallenge = new SignedChallenge(serializedChallenge, challengeSignature);

        var createIdentityPayload = new CreateIdentityRequest
        {
            ClientId = CLIENT_ID,
            ClientSecret = CLIENT_SECRET,
            IdentityVersion = 1,
            SignedChallenge = signedChallenge,
            IdentityPublicKey = ConvertibleString.FromUtf8(JsonConvert.SerializeObject(new CryptoSignaturePublicKey
            {
                alg = CryptoExchangeAlgorithm.ECDH_X25519,
                pub = identityKeyPair.PublicKey.Base64Representation
            })).BytesRepresentation,
            DevicePassword = DEVICE_PASSWORD
        };

        _responseContext.WhenResponse = _responseContext.CreateIdentityResponse = await AnonymousClient.Identities.CreateIdentity(createIdentityPayload);
    }

    [When(@"(.+) sends a POST request to the /Identities/Self/DeletionProcesses endpoint")]
    public async Task WhenIdentitySendsAPOSTRequestToTheIdentitiesSelfDeletionProcessesEndpoint(string identityName)
    {
        _responseContext.WhenResponse = _responseContext.StartDeletionProcessResponse = await Identity(identityName).Identities.StartDeletionProcess();
    }
    #endregion

    private static List<string> SplitNames(string identityNames)
    {
        return identityNames.Split([", ", " and "], StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}

public class IdentitiesContext
{
    public Client? AnonymousClient { get; set; }
    public readonly Dictionary<string, Client> Identities = new();
}

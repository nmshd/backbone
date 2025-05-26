using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.FeatureFlags.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Backbone.UnitTestTools.Shouldly.Extensions;
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
    private ApiResponse<GetFeatureFlagsResponse>? _getFeatureFlagsResponse;

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

    [Given($@"{RegexFor.SINGLE_THING} has feature flags feature1 (enabled|disabled) and feature2 (enabled|disabled)")]
    public async Task GivenIHasFeatureFlagsFeatureEnabledAndFeatureDisabled(string identityName, string feature1State, string feature2State)
    {
        var featureFlags = new ChangeFeatureFlagsRequest
        {
            { "feature1", feature1State == "enabled" },
            { "feature2", feature2State == "enabled" }
        };

        var client = _clientPool.FirstForIdentityName(identityName);
        await client.FeatureFlags.ChangeFeatureFlags(featureFlags);
    }

    [Given($@"{RegexFor.SINGLE_THING} has \d+ feature flags with name feature\[(\d+)\.\.\.(\d+)]")]
    public async Task GivenIHasFeatureFlagsWithNameFeature(string identityName, int lowerBoundNamePostfix, int upperBoundNamePostfix)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var featureFlags = new ChangeFeatureFlagsRequest();
        for (var i = lowerBoundNamePostfix; i <= upperBoundNamePostfix; i++)
        {
            featureFlags.Add($"feature{i}", true);
        }

        await client.FeatureFlags.ChangeFeatureFlags(featureFlags);
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

    [When($@"an anonymous user sends a GET request to the /Identities/IsDeleted endpoint with {RegexFor.SINGLE_THING}.Username")]
    public async Task WhenAnAnonymousUserSendsAGETRequestToTheIdentitiesIsDeletedEndpointWithDUsername(string deviceName)
    {
        var client = _clientPool.GetForDeviceName(deviceName);

        var device = await client.Devices.GetActiveDevice();

        _responseContext.WhenResponse = _isDeletedResponse = await _clientPool.Anonymous.Identities.IsDeleted(device.Result!.Username);
    }

    [When($@"{RegexFor.SINGLE_THING} sends a PATCH request to the /Identities/Self/FeatureFlags endpoint with feature1 (enabled|disabled) and feature2 (enabled|disabled)")]
    public async Task WhenISendsAPatchRequestToTheIdentitiesSelfFeatureFlagsEndpointWithFeature1EnabledAndFeature2Disabled(string identityName, string feature1State, string feature2State)
    {
        var featureFlags = new ChangeFeatureFlagsRequest
        {
            { "feature1", feature1State == "enabled" },
            { "feature2", feature2State == "enabled" }
        };

        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = await client.FeatureFlags.ChangeFeatureFlags(featureFlags);
    }

    [When($@"^{RegexFor.SINGLE_THING} sends a GET request to the /Identities/{{address}}/FeatureFlags endpoint with address={RegexFor.SINGLE_THING}.address$")]
    public async Task WhenISendsAGETRequestToTheIdentitiesAddressFeatureFlagsEndpointWithAddressIAddress(string requestorName, string peerName)
    {
        var requestorClient = _clientPool.FirstForIdentityName(requestorName);

        var peerAddress = _clientPool.FirstForIdentityName(peerName).IdentityData!.Address;
        _responseContext.WhenResponse = _getFeatureFlagsResponse = await requestorClient.FeatureFlags.GetFeatureFlags(peerAddress);
    }

    [When($@"{RegexFor.SINGLE_THING} sends a PATCH request to the /Identities/Self/FeatureFlags endpoint with (\d*) features")]
    public async Task WhenISendsApatchRequestToTheIdentitiesSelfFeatureFlagsEndpointWithFeatures(string identityName, int numberOfFeatureFlags)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var featureFlags = new ChangeFeatureFlagsRequest();

        for (var i = 0; i < numberOfFeatureFlags; i++)
        {
            featureFlags.Add($"feature{i}", true);
        }

        _responseContext.WhenResponse = await client.FeatureFlags.ChangeFeatureFlags(featureFlags);
    }

    [When($@"{RegexFor.SINGLE_THING} sends a PATCH request to the /Identities/Self/FeatureFlags endpoint with 1 feature flag with name feature(\d+)")]
    public async Task WhenISendsApatchRequestToTheIdentitiesSelfFeatureFlagsEndpointWithFeatureFlagWithNameFeature(string identityName, int namePostfix)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var featureFlags = new ChangeFeatureFlagsRequest
        {
            { $"feature{namePostfix}", true }
        };

        _responseContext.WhenResponse = await client.FeatureFlags.ChangeFeatureFlags(featureFlags);
    }

    #endregion

    #region Then

    [Then(@"the response says that the identity was not deleted")]
    public void ThenTheResponseSaysThatTheIdentityWasNotDeleted()
    {
        _isDeletedResponse!.Result!.IsDeleted.ShouldBeFalse();
    }

    [Then(@"the deletion date is not set")]
    public void ThenTheDeletionDateIsNotSet()
    {
        _isDeletedResponse!.Result!.DeletionDate.ShouldBeNull();
    }

    [Then($@"the Backbone has persisted feature1 as (enabled|disabled) and feature2 as (enabled|disabled) for {RegexFor.SINGLE_THING}")]
    public async Task ThenTheBackboneHasPersistedFeatureAsEnabledAndFeatureAsDisabled(string feature1State, string feature2State, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var response = await client.FeatureFlags.GetFeatureFlags(client.IdentityData!.Address);

        response.IsSuccess.ShouldBeTrue();

        response.Result!.ShouldHaveCount(2);
        response.Result!.First(kv => kv.Key == "feature1").Value.ShouldBe(feature1State == "enabled");
        response.Result!.First(kv => kv.Key == "feature2").Value.ShouldBe(feature2State == "enabled");
    }

    [Then(@"^the response contains the feature flags feature1 (enabled|disabled) and feature2 (enabled|disabled)$")]
    public void ThenTheResponseContainsTheFeatureFlagsFeatureEnabledAndFeatureDisabled(string feature1State, string feature2State)
    {
        _getFeatureFlagsResponse.ShouldNotBeNull();

        _getFeatureFlagsResponse!.Result!.ShouldHaveCount(2);
        _getFeatureFlagsResponse!.Result!.First(kv => kv.Key == "feature1").Value.ShouldBe(feature1State == "enabled");
        _getFeatureFlagsResponse!.Result!.First(kv => kv.Key == "feature2").Value.ShouldBe(feature2State == "enabled");
    }

    [Then($@"{RegexFor.SINGLE_THING} has no feature flags")]
    public async Task ThenIHasNoFeatureFlags(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var response = await client.FeatureFlags.GetFeatureFlags(client.IdentityData!.Address);

        response.IsSuccess.ShouldBeTrue();
        response.Result!.ShouldBeEmpty();
    }

    [Then($@"{RegexFor.SINGLE_THING} has \d+ feature flags with names feature\[(\d+)\.\.\.(\d+)]")]
    public async Task ThenIHasFeatureFlagsWithNamesFeature(string identityName, int lowerBoundNamePostfix, int upperBoundNamePostfix)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var response = await client.FeatureFlags.GetFeatureFlags(client.IdentityData!.Address);

        response.IsSuccess.ShouldBeTrue();

        for (var i = lowerBoundNamePostfix; i <= upperBoundNamePostfix; i++)
            response.Result!.ShouldContainKey($"feature{i}");
    }

    #endregion
}

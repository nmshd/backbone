using Backbone.AdminUi.Tests.Integration.API;
using Backbone.AdminUi.Tests.Integration.Extensions;
using Backbone.AdminUi.Tests.Integration.Models;
using Backbone.Crypto;
using Backbone.Crypto.Abstractions;
using Newtonsoft.Json;

namespace Backbone.AdminUi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Identities")]
[Scope(Feature = "POST Identities/{id}/DeletionProcess")]
internal class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private readonly IdentitiesApi _identitiesApi;
    private readonly ISignatureHelper _signatureHelper;
    private ODataResponse<List<IdentityOverviewDTO>>? _identityOverviewsResponse;
    private HttpResponse<IdentitySummaryDTO>? _identityResponse;
    private HttpResponse<CreateIdentityResponse>? _createIdentityResponse;
    private HttpResponse<StartDeletionProcessAsSupportResponse>? _identityDeletionProcessResponse;
    private string _existingIdentity;

    public IdentitiesApiStepDefinitions(IdentitiesApi identitiesApi, ISignatureHelper signatureHelper)
    {
        _identitiesApi = identitiesApi;
        _signatureHelper = signatureHelper;
        _existingIdentity = string.Empty;
    }


    [Given("an active deletion process for Identity i exists")]
    public async Task GivenAnActiveDeletionProcessForIdentityAExists()
    {
        await _identitiesApi.StartDeletionProcess(_createIdentityResponse!.Content.Result!.Address, _requestConfiguration);
    }

    [Given(@"an Identity i")]
    public async Task GivenAnIdentityI()
    {
        var keyPair = _signatureHelper.CreateKeyPair();

        dynamic publicKey = new
        {
            pub = keyPair.PublicKey.Base64Representation,
            alg = 3
        };

        var createIdentityRequest = new CreateIdentityRequest()
        {
            ClientId = "test",
            ClientSecret = "test",
            DevicePassword = "test",
            IdentityPublicKey = (ConvertibleString.FromUtf8(JsonConvert.SerializeObject(publicKey)) as ConvertibleString)!.Base64Representation,
            IdentityVersion = 1,
            SignedChallenge = new CreateIdentityRequestSignedChallenge
            {
                Challenge = "string.Empty",
                Signature = "some-dummy-signature"
            }
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createIdentityRequest);

        _createIdentityResponse = await _identitiesApi.CreateIdentity(requestConfiguration);
        _createIdentityResponse.IsSuccessStatusCode.Should().BeTrue();
        _existingIdentity = _createIdentityResponse.Content.Result!.Address;
    }

    [When("a POST request is sent to the /Identities/{i.id}/DeletionProcesses endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesIdDeletionProcessesEndpoint()
    {
        _identityDeletionProcessResponse = await _identitiesApi.StartDeletionProcess(_createIdentityResponse!.Content.Result!.Address, _requestConfiguration);
    }

    [When(@"a GET request is sent to the /Identities endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesOverviewEndpoint()
    {
        _identityOverviewsResponse = await _identitiesApi.GetIdentityOverviews(_requestConfiguration);
        _identityOverviewsResponse.Should().NotBeNull();
        _identityOverviewsResponse!.Content.Should().NotBeNull();
    }

    [When(@"a GET request is sent to the /Identities/{i.address} endpoint")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpoint()
    {
        _identityResponse = await _identitiesApi.GetIdentityByAddress(_requestConfiguration, _existingIdentity);
        _identityResponse.Should().NotBeNull();
        _identityResponse.Content.Should().NotBeNull();
    }

    [When(@"a GET request is sent to the /Identities/{address} endpoint with an inexistent address")]
    public async Task WhenAGETRequestIsSentToTheIdentitiesAddressEndpointForAnInexistentIdentity()
    {
        _identityResponse = await _identitiesApi.GetIdentityByAddress(_requestConfiguration, "inexistentIdentityAddress");
        _identityResponse.Should().NotBeNull();
        _identityResponse.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a list of Identities")]
    public void ThenTheResponseContainsAListOfIdentities()
    {
        _identityOverviewsResponse!.Content.Value.Should().NotBeNull();
        _identityOverviewsResponse!.Content.Value.Should().NotBeNullOrEmpty();
        _identityOverviewsResponse!.AssertContentTypeIs("application/json");
        _identityOverviewsResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response contains a Deletion Process")]
    public void ThenTheResponseContainsADeletionProcess()
    {
        _identityDeletionProcessResponse!.Content.Result.Should().NotBeNull();
        _identityDeletionProcessResponse!.AssertContentTypeIs("application/json");
        _identityDeletionProcessResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response contains Identity i")]
    public void ThenTheResponseContainsAnIdentity()
    {
        _identityResponse!.AssertHasValue();
        _identityResponse!.AssertStatusCodeIsSuccess();
        _identityResponse!.AssertContentTypeIs("application/json");
        _identityResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_identityResponse != null)
        {
            var actualStatusCode = (int)_identityResponse!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_identityOverviewsResponse != null)
        {
            var actualStatusCode = (int)_identityOverviewsResponse!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_identityDeletionProcessResponse != null)
        {
            var actualStatusCode = (int)_identityDeletionProcessResponse!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_identityResponse != null)
        {
            _identityResponse!.Content.Error.Should().NotBeNull();
            _identityResponse.Content.Error!.Code.Should().Be(errorCode);
        }

        if (_identityDeletionProcessResponse != null)
        {
            _identityDeletionProcessResponse!.Content.Error.Should().NotBeNull();
            _identityDeletionProcessResponse.Content.Error!.Code.Should().Be(errorCode);
        }
    }
}

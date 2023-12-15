using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto.Abstractions;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Identities/Self/DeletionProcess")]
[Scope(Feature = "POST Identity")]
internal class IdentitiesApiStepDefinitions : BaseStepDefinitions
{
    private HttpResponse<StartDeletionProcessResponse>? _response;
    private HttpResponse<CreateIdentityResponse>? _identityResponse;
    private HttpResponse<Challenge>? _challengeResponse;

    public IdentitiesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, IdentitiesApi identitiesApi, ChallengesApi challengesApi, ISignatureHelper signatureHelper, DevicesApi devicesApi) :
        base(httpConfiguration, signatureHelper, challengesApi, identitiesApi, devicesApi)
    { }

    [Given("no active deletion process for the identity exists")]
    public void GivenNoActiveDeletionProcessForTheUserExists()
    {
    }

    [Given("an active deletion process for the identity exists")]
    public async Task GivenAnActiveDeletionProcessForTheUserExists()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.AuthenticationParameters.Username = "USRa";
        requestConfiguration.AuthenticationParameters.Password = "a";

        await _identitiesApi.StartDeletionProcess(requestConfiguration);
    }

    [When("a POST request is sent to the /Identities/Self/DeletionProcesses endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesSelfDeletionProcessEndpoint()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.AuthenticationParameters.Username = "USRa";
        requestConfiguration.AuthenticationParameters.Password = "a";

        _response = await _identitiesApi.StartDeletionProcess(requestConfiguration);
    }

    [Then(@"the response content includes an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _response!.Content.Should().NotBeNull();
        _response.Content!.Error.Should().NotBeNull();
        _response.Content.Error!.Code.Should().Be(errorCode);
    }

    [Then("the response contains a Deletion Process")]
    public void ThenTheResponseContainsADeletionProcess()
    {
        _response!.Content.Should().NotBeNull();
        _response!.Content.Result.Should().NotBeNull();
        _response!.AssertContentCompliesWithSchema();
    }

    [Given(@"a Challenge c")]
    public async Task GivenAChallengeC()
    {
        _challengeResponse = await CreateChallenge();
    }

    [When(@"a POST request is sent to the /Identities endpoint with a valid signature on c")]
    public async Task WhenAPOSTRequestIsSentToTheIdentitiesEndpoint()
    {
        _identityResponse = await CreateIdentity(_challengeResponse!.Content.Result);
    }

    [Given(@"an Identity i")]
    public async Task GivenAnIdentityI()
    {
        _challengeResponse = await CreateChallenge();
        _identityResponse = await CreateIdentity(_challengeResponse.Content.Result);
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
        if (_identityResponse != null)
        {
            var actualStatusCode = (int)_identityResponse!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_response != null)
        {
            var actualStatusCode = (int)_response!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }
    }
}

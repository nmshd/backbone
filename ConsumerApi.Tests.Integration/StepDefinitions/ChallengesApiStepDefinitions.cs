using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Challenge")]
[Scope(Feature = "GET Challenge")]
internal class ChallengesApiStepDefinitions
{
    private string _challengeId;
    private ApiResponse<Challenge>? _response;
    private Client _sdk = null!;
    private bool _isAuthenticated;
    private readonly HttpClient _httpClient;
    private readonly ClientCredentials _clientCredentials;

    public ChallengesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, HttpClientFactory factory)
    {
        _challengeId = string.Empty;
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

    [Given("the user is authenticated")]
    public async Task GivenTheUserIsAuthenticated()
    {
        _sdk = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
        _isAuthenticated = true;
    }

    [Given("the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _sdk = Client.CreateUnauthenticated(_httpClient, _clientCredentials);
        _isAuthenticated = false;
    }

    [Given("a Challenge c")]
    public async Task GivenAChallengeC()
    {
        var challengeResponse = !_isAuthenticated ? await _sdk.Challenges.CreateChallengeUnauthenticated() : await _sdk.Challenges.CreateChallenge();
        challengeResponse.Should().BeASuccess();
        _challengeId = challengeResponse.Result!.Id;

        _challengeId.Should().NotBeNullOrEmpty();
    }

    [When("a POST request is sent to the Challenges endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpoint()
    {
        _response = _isAuthenticated ? await _sdk.Challenges.CreateChallenge() : await _sdk.Challenges.CreateChallengeUnauthenticated();
    }

    [When(@"a GET request is sent to the Challenges/{id} endpoint with ""?(.*?)""?")]
    public async Task WhenAGETRequestIsSentToTheChallengesIdEndpointWith(string id)
    {
        switch (id)
        {
            case "c.Id":
                id = _challengeId;
                break;
            case "a valid Id":
                id = "CHLjVPS6h1082AuBVBaR";
                break;
        }

        _response = await _sdk.Challenges.GetChallenge(id);
    }

    [Then("the response contains a Challenge")]
    public void ThenTheResponseContainsAChallenge()
    {
        _response!.Should().NotBeNull();
        _response!.Should().BeASuccess();
        _response!.ContentType.Should().Be("application/json");
        _response.Should().ComplyWithSchema();
        AssertExpirationDateIsInFuture();
    }

    [Then("the Challenge does not contain information about the creator")]
    public void ThenTheChallengeDoesNotContainInformationAboutTheCreator()
    {
        _response!.Result!.CreatedBy.Should().BeNull();
        _response.Result!.CreatedByDevice.Should().BeNull();
    }

    [Then("the Challenge contains information about the creator")]
    public void ThenTheChallengeContainsInformationAboutTheCreator()
    {
        _response!.Result!.CreatedBy.Should().NotBeNull();
        _response.Result!.CreatedByDevice.Should().NotBeNull();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.Status;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _response!.Error.Should().NotBeNull();
        _response.Error!.Code.Should().Be(errorCode);
    }

    private void AssertExpirationDateIsInFuture()
    {
        _response!.Result!.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}

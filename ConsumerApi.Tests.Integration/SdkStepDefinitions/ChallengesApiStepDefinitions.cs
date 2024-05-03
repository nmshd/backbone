using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.SdkStepDefinitions;

[Binding]
[Scope(Feature = "POST Challenge")]
[Scope(Feature = "GET Challenge")]
internal class ChallengesApiStepDefinitions
{
    private Client? _client;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;
    private ApiResponse<Challenge>? _response;
    private string _challengeId;

    public ChallengesApiStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
        _challengeId = string.Empty;
    }

    [Given("the user is authenticated")]
    public async Task GivenTheUserIsAuthenticated()
    {
        _client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, "somePassword");
    }

    [Given("the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _client = Client.CreateUnauthenticated(_httpClient, _clientCredentials);
    }

    [Given("a Challenge c")]
    public async Task GivenAChallengeC()
    {
        var challengeResponse = await _client!.Challenges.CreateChallenge();
        challengeResponse.IsSuccess.Should().BeTrue();

        _challengeId = challengeResponse.Result!.Id;
        _challengeId.Should().NotBeNullOrEmpty();
    }

    [When("a POST request is sent to the Challenges endpoint without authentication")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpointWithoutAuthentication()
    {
        _response = await _client!.Challenges.CreateChallengeUnauthenticated();
    }

    [When("a POST request is sent to the Challenges endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpoint()
    {
        _response = await _client!.Challenges.CreateChallenge();
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

        _response = await _client!.Challenges.GetChallenge(id);
    }

    [Then("the response contains a Challenge")]
    public void ThenTheResponseContainsAChallenge()
    {
        _response!.Should().NotBeNull();
        _response!.IsSuccess.Should().BeTrue();
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
        ((int)_response!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
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

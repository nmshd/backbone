using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Services;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Challenge")]
[Scope(Feature = "GET Challenge")]
internal class ChallengesApiStepDefinitions
{
    private string _challengeId;
    private ApiResponse<Challenge>? _response;
    private Client _sdk;
    private readonly Sdk.Configuration _baseConfig;
    private readonly HttpClient _httpClient;

    public ChallengesApiStepDefinitions(HttpClientFactory factory)
    {
        _challengeId = string.Empty;

        _httpClient = factory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5000/api/v1/");
        _baseConfig = new Sdk.Configuration
        {
            BaseUrl = null!,
            Authentication = new Sdk.Configuration.AuthenticationConfiguration
            {
                ClientId = "test",//httpConfiguration.Value.ClientCredentials.ClientId,
                ClientSecret = "test",//.Value.ClientCredentials.ClientSecret,
                Username = "",
                Password = ""
            }
        };

        _sdk = new Client(_httpClient, _baseConfig);
    }

    [Given("the user is authenticated")]
    public async Task AuthenticatedUser()
    {
        var accountController = new AccountController(_sdk);
        var createdIdentity = await accountController.CreateIdentity(_baseConfig.Authentication.ClientId, _baseConfig.Authentication.ClientSecret) ?? throw new InvalidOperationException();
        var authenticatedConfig = _baseConfig.CloneWith(new Sdk.Configuration.AuthenticationConfiguration
        {
            Username = createdIdentity.DeviceUsername,
            Password = createdIdentity.DevicePassword,
            ClientId = _baseConfig.Authentication.ClientId,
            ClientSecret = _baseConfig.Authentication.ClientSecret,

        }) as Sdk.Configuration;

        _sdk = new Client(_httpClient, authenticatedConfig!);
    }

    [Given("the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        var nonAuthenticatedConfig = _baseConfig.CloneWith(new Sdk.Configuration.AuthenticationConfiguration
        {
            Username = "",
            Password = "",
            ClientId = _baseConfig.Authentication.ClientId,
            ClientSecret = _baseConfig.Authentication.ClientSecret,

        }) as Sdk.Configuration;

        _sdk = new Client(_httpClient, nonAuthenticatedConfig!);
    }


    [Given("a Challenge c")]
    public async Task GivenAChallengeC()
    {
        var challengeResponse = await _sdk.Challenges.CreateChallenge();
        challengeResponse.IsSuccess.Should().BeTrue();

        _challengeId = challengeResponse.Result!.Id;
        _challengeId.Should().NotBeNullOrEmpty();
    }

    [When("a POST request is sent to the Challenges endpoint with")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpointWith(Table table)
    {
        _response = await _sdk.Challenges.CreateChallenge();
    }

    [When("a POST request is sent to the Challenges endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpoint()
    {
        _response = await _sdk.Challenges.CreateChallenge();
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
        _response!.IsSuccess.Should().BeTrue();
        _response!.ContentType.Should().StartWith("application/json");
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

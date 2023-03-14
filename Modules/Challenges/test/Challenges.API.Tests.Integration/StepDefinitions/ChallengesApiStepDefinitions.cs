using Challenges.API.Tests.Integration.API;
using Challenges.API.Tests.Integration.Extensions;
using Challenges.API.Tests.Integration.Models;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow.Assist;
using static Challenges.API.Tests.Integration.Configuration.Settings;

namespace Challenges.API.Tests.Integration.StepDefinitions;

[Binding]
public class ChallengesApiStepDefinitions
{
    private readonly ChallengesApi _challengeApi;
    private string _challengeId;
    private HttpResponse<ChallengeResponse> _challengeResponse;
    private readonly RequestConfiguration _requestConfiguration;

    public ChallengesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, ChallengesApi challengeApi)
    {
        _challengeApi = challengeApi;
        _challengeId = string.Empty;
        _challengeResponse = new HttpResponse<ChallengeResponse>();
        _requestConfiguration = new RequestConfiguration
        {
            AuthenticationParameters = new AuthenticationParameters
            {
                GrantType = "password",
                ClientId = httpConfiguration.Value.ClientCredentials.ClientId,
                ClientSecret = httpConfiguration.Value.ClientCredentials.ClientSecret,
                UserName = "USRa",
                Password = "a"
            }
        };
    }

    [Given(@"the user is authenticated")]
    public void GivenTheUserIsAuthenticated()
    {
        _requestConfiguration.IsAuthenticated = true;
    }

    [Given(@"the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _requestConfiguration.IsAuthenticated = false;
    }

    [Given(@"the Accept header is '([^']*)'")]
    public void GivenTheAcceptHeaderIs(string acceptHeader)
    {
        _requestConfiguration.AcceptHeader = acceptHeader;
    }

    [Given(@"a Challenge c")]
    public async Task GivenAChallengeC()
    {
        var challengeResponse = await _challengeApi.CreateChallenge(_requestConfiguration);
        challengeResponse.IsSuccessStatusCode.Should().BeTrue();

        _challengeId = challengeResponse.Data!.Result!.Id;
        _challengeId.Should().NotBeNullOrEmpty(because: "Required value for 'Id' is missing.");
    }

    [When(@"a POST request is sent to the Challenges endpoint with")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpointWith(Table table)
    {
        var requestConfiguration = table.CreateInstance<RequestConfiguration>();
        _challengeResponse = await _challengeApi.CreateChallenge(requestConfiguration);
    }

    [When(@"a POST request is sent to the Challenges endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpoint()
    {
        _challengeResponse = await _challengeApi.CreateChallenge(_requestConfiguration);
    }

    [When(@"a GET request is sent to the Challenges/{id} endpoint with ""?(.*?)""?")]
    public async Task WhenAGETRequestIsSentToTheChallengesIdEndpointWith(string id)
    {
        switch (id)
        {
            case "c.Id":
                id = _challengeId!;
                break;
            case "a valid Id":
                id = "CHLjVPS6h1082AuBVBaR";
                break;
        }
        _challengeResponse = await _challengeApi.GetChallengeById(_requestConfiguration, id);
    }

    [Then(@"the response content includes an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _challengeResponse.Data!.Error.Should().NotBeNull();

        _challengeResponse.Data!.Error!.Code.Should().Be(errorCode);
    }

    [Then(@"the response contains a Challenge")]
    public void ThenTheResponseContainsAChallenge()
    {
        _challengeResponse.Should().NotBeNull();

        AssertStatusCodeIsSuccess();

        AssertResponseContentTypeIsJson();

        AssertResponseBodyCompliesWithSchema();

        AssertExpirationDateIsInFuture();
    }

    [Then(@"the Challenge does not contain information about the creator")]
    public void ThenTheChallengeDoesNotContainInformationAboutTheCreator()
    {
        _challengeResponse.Data!.Result!.CreatedBy.Should().BeNull();

        _challengeResponse.Data!.Result!.CreatedByDevice.Should().BeNull();
    }

    [Then(@"the Challenge contains information about the creator")]
    public void ThenTheChallengeContainsInformationAboutTheCreator()
    {
        _challengeResponse.Data!.Result!.CreatedBy.Should().NotBeNull();

        _challengeResponse.Data!.Result!.CreatedByDevice.Should().NotBeNull();
    }


    [Then(@"the response status code is (.*)")]
    public void ThenTheResponseStatusCodeIs(int statusCode)
    {
        ((int)_challengeResponse.StatusCode).Should().Be(statusCode);
    }

    private void AssertStatusCodeIsSuccess()
    {
        _challengeResponse.IsSuccessStatusCode.Should().BeTrue();
    }

    private void AssertResponseContentTypeIsJson()
    {
        _challengeResponse.ContentType.Should().Be("application/json");
    }

    private void AssertResponseBodyCompliesWithSchema()
    {
        JsonValidators.ValidateJsonSchema<ChallengeResponse>(_challengeResponse.Content!, out var errors)
            .Should().BeTrue($"Response body does not comply with the Challenge schema: {string.Join(", ", errors)}");
    }

    private void AssertExpirationDateIsInFuture()
    {
        _challengeResponse.Data!.Result!.ExpiresAt.Should().BeAfter(DateTime.Now);
    }
}

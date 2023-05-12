using ConsumerApi.Tests.Integration.API;
using ConsumerApi.Tests.Integration.Configuration;
using ConsumerApi.Tests.Integration.Extensions;
using ConsumerApi.Tests.Integration.Models;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow.Assist;

namespace ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Challenge")]
[Scope(Feature = "GET Challenge")]
public class ChallengesApiStepDefinitions : BaseStepDefinitions
{
    private readonly ChallengesApi _challengeApi;
    private string _challengeId;
    private HttpResponse<Challenge>? _response;

    public ChallengesApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, ChallengesApi challengeApi) : base(httpConfiguration)
    {
        _challengeApi = challengeApi;
        _challengeId = string.Empty;
    }

    [Given(@"a Challenge c")]
    public async Task GivenAChallengeC()
    {
        var challengeResponse = await _challengeApi.CreateChallenge(_requestConfiguration);
        challengeResponse.IsSuccessStatusCode.Should().BeTrue();

        _challengeId = challengeResponse.Content.Result!.Id;
        _challengeId.Should().NotBeNullOrEmpty();
    }

    [When(@"a POST request is sent to the Challenges endpoint with")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpointWith(Table table)
    {
        var requestConfiguration = table.CreateInstance<RequestConfiguration>();
        requestConfiguration.SupplementWith(_requestConfiguration);

        _response = await _challengeApi.CreateChallenge(requestConfiguration);
    }

    [When(@"a POST request is sent to the Challenges endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpoint()
    {
        _response = await _challengeApi.CreateChallenge(_requestConfiguration);
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
        _response = await _challengeApi.GetChallengeById(_requestConfiguration, id);
    }

    [Then(@"the response contains a Challenge")]
    public void ThenTheResponseContainsAChallenge()
    {
        _response!.AssertHasValue();
        _response!.AssertStatusCodeIsSuccess();
        _response!.AssertContentTypeIs("application/json");

        AssertExpirationDateIsInFuture();
    }

    [Then(@"the Challenge does not contain information about the creator")]
    public void ThenTheChallengeDoesNotContainInformationAboutTheCreator()
    {
        _response!.Content.Result!.CreatedBy.Should().BeNull();
        _response.Content.Result!.CreatedByDevice.Should().BeNull();
    }

    [Then(@"the Challenge contains information about the creator")]
    public void ThenTheChallengeContainsInformationAboutTheCreator()
    {
        _response!.Content.Result!.CreatedBy.Should().NotBeNull();
        _response.Content.Result!.CreatedByDevice.Should().NotBeNull();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _response!.Content.Error.Should().NotBeNull();
        _response.Content.Error!.Code.Should().Be(errorCode);
    }

    private void AssertExpirationDateIsInFuture()
    {
        _response!.Content.Result!.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}

using Challenges.API.Tests.Integration.API;
using Challenges.API.Tests.Integration.Extensions;
using Challenges.API.Tests.Integration.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using TechTalk.SpecFlow.Assist;
using static Challenges.API.Tests.Integration.Configuration.Settings;

namespace Challenges.API.Tests.Integration.StepDefinitions;

[Binding]
public class ChallengesApiStepDefinitions
{
    private readonly IConfiguration _config;
    private readonly ChallengesApi _challengeApi;
    private string _challengeId;
    private bool _isAuthenticatedChallenge;
    private RestResponse<ChallengeResponse> _challengeResponse;
    private readonly RequestConfiguration _requestConfiguration;
    private readonly AuthenticationParameters _authenticationParams;

    public ChallengesApiStepDefinitions(IConfiguration config, ChallengesApi challengeApi)
    {
        _config = config;
        var settings = _config.GetSection("Http").Get<HttpConfiguration>() ?? new HttpConfiguration();
        _challengeApi = challengeApi;
        _challengeId = string.Empty;
        _challengeResponse = new RestResponse<ChallengeResponse>();
        _requestConfiguration = new RequestConfiguration();
        _authenticationParams = new AuthenticationParameters
        {
            Parameters = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", settings.ClientCredentials.ClientId },
                { "client_secret", settings.ClientCredentials.ClientSecret },
                { "username", "USRa" },
                { "password", "a" }
            }
        };
    }

    [Given(@"the user is authenticated")]
    public async Task GivenTheUserIsAuthenticated()
    {
        var accessTokenResponse = await _challengeApi.GetAccessTokenAsync(_authenticationParams);
        _requestConfiguration.TokenResponse = accessTokenResponse.Data;

        accessTokenResponse.IsSuccessStatusCode.Should().BeTrue();
    }

    [Given(@"the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _requestConfiguration.TokenResponse = null;
    }

    [Given(@"the Accept header is '([^']*)'")]
    public void GivenTheAcceptHeaderIs(string acceptHeader)
    {
        _requestConfiguration.AcceptHeader = acceptHeader;
    }

    [Given(@"a Challenge c")]
    public async Task GivenAChallengeC()
    {
        var challengeResponse = await _challengeApi.CreateChallengeAsync(new RequestConfiguration());
        challengeResponse.IsSuccessStatusCode.Should().BeTrue();

        _challengeId = challengeResponse.Data!.Result.Id;
        _challengeId.Should().NotBeNullOrEmpty(because: "Required value for 'Id' is missing.");
    }

    [When(@"a POST request is sent to the Challenges endpoint with")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpointWith(Table table)
    {
        // Check if the request was made by an authenticated user
        _isAuthenticatedChallenge = !string.IsNullOrEmpty(_requestConfiguration.TokenResponse?.AccessToken);

        var requestConfiguration = table.CreateInstance<RequestConfiguration>();
        _challengeResponse = await _challengeApi.CreateChallengeAsync(requestConfiguration);
    }

    [When(@"a POST request is sent to the Challenges endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheChallengesEndpoint()
    {
        // Check if the request was made by an authenticated user
        _isAuthenticatedChallenge = !string.IsNullOrEmpty(_requestConfiguration.TokenResponse?.AccessToken);

        _challengeResponse = await _challengeApi.CreateChallengeAsync(_requestConfiguration);
    }

    [When(@"a GET request is sent to the Challenges/\{id} endpoint with ""([^""]*)""")]
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
        _challengeResponse = await _challengeApi.GetChallengeByIdAsync(_requestConfiguration, id);
    }

    [Then(@"the response content includes an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _challengeResponse.Content.Should().NotBeNull();

        var error = JsonConvert.DeserializeObject<ErrorResponse>(_challengeResponse.Content!);

        error.Should().NotBeNull();
        error!.Error.Code.Should().Be(errorCode);
    }

    [Then(@"the response contains a Challenge")]
    public void ThenTheResponseContainsAChallenge()
    {
        _challengeResponse.Data.Should().NotBeNull();

        AssertStatusCodeIsSuccess();

        AssertResponseContentTypeIsJson();

        AssertResponseBodyCompliesWithSchema();

        AssertExpirationDateIsInFuture();

        // Check if Challenge was created by an authenticated user
        if (_isAuthenticatedChallenge)
        {
            AssertCreatedByIsNotNull();
            AssertCreatedByDeviceIsNotNull();
        }
        else
        {
            AssertCreatedByIsNull();
            AssertCreatedByDeviceIsNull();
        }
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
        _challengeResponse.Data!.Result.ExpiresAt.Should().BeAfter(DateTime.Now);
    }

    private void AssertCreatedByIsNotNull()
    {
        _challengeResponse.Data!.Result.CreatedBy.Should().NotBeNull();
    }

    private void AssertCreatedByDeviceIsNotNull()
    {
        _challengeResponse.Data!.Result.CreatedByDevice.Should().NotBeNull();
    }

    private void AssertCreatedByIsNull()
    {
        _challengeResponse.Data!.Result.CreatedBy.Should().BeNull();
    }

    private void AssertCreatedByDeviceIsNull()
    {
        _challengeResponse.Data!.Result.CreatedByDevice.Should().BeNull();
    }
}

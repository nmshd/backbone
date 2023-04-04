using System.Net;
using Microsoft.Extensions.Options;
using RestSharp;
using TechTalk.SpecFlow.Assist;
using Tokens.API.Tests.Integration.API;
using Tokens.API.Tests.Integration.Extensions;
using Tokens.API.Tests.Integration.Models;
using static Tokens.API.Tests.Integration.Configuration.Settings;

namespace Tokens.API.Tests.Integration.StepDefinitions;

[Binding]
public class TokensApiStepDefinitions
{
    private readonly TokensApi _tokensApi;
    private const int TOKEN_ITERATIONS = 2;
    private string _tokenId;
    private string _peerTokenId;
    private readonly List<Token> _createdTokens;
    private readonly List<Token> _responseTokens;
    private HttpResponse<TokenResponse<Token>> _tokenResponse;
    private readonly RequestConfiguration _requestConfiguration;

    public TokensApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, TokensApi tokensApi)
    {
        _tokensApi = tokensApi;
        _tokenId = string.Empty;
        _peerTokenId = string.Empty;
        _createdTokens = new List<Token>();
        _responseTokens = new List<Token>();
        _tokenResponse = new HttpResponse<TokenResponse<Token>>();
        _requestConfiguration = new RequestConfiguration
        {
            AuthenticationParameters = new AuthenticationParameters
            {
                GrantType = "password",
                ClientId = httpConfiguration.Value.ClientCredentials.ClientId,
                ClientSecret = httpConfiguration.Value.ClientCredentials.ClientSecret,
                Username = "USRa",
                Password = "a"
            }
        };
    }

    [Given(@"the user is authenticated")]
    public void GivenTheUserIsAuthenticated()
    {
        _requestConfiguration.Authenticate = true;
    }

    [Given(@"the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _requestConfiguration.Authenticate = false;
    }

    [Given(@"the Accept header is '([^']*)'")]
    public void GivenTheAcceptHeaderIs(string acceptHeader)
    {
        _requestConfiguration.AcceptHeader = acceptHeader;
    }

    [Given(@"an own Token t")]
    public async Task GivenAnOwnTokenT()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Content = "QQ==";
        requestConfiguration.ExpiresAt = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
        requestConfiguration.Authenticate = true;

        var httpResponse = await _tokensApi.CreateToken(requestConfiguration);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();

        var token = httpResponse.Data!.Result!;

        _tokenId = token.Id;
        _tokenId.Should().NotBeNullOrEmpty(because: "Required value for 'Id' is missing.");
    }

    [Given(@"a peer Token p")]
    public async Task GivenAPeerTokenP()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.AuthenticationParameters.Username = "USRb";
        requestConfiguration.AuthenticationParameters.Password = "b";
        requestConfiguration.Content = "QQ==";
        requestConfiguration.ExpiresAt = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
        requestConfiguration.Authenticate = true;

        var httpResponse = await _tokensApi.CreateToken(requestConfiguration);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();

        var token = httpResponse.Data!.Result!;

        _peerTokenId = token.Id!;
        _peerTokenId.Should().NotBeNullOrEmpty(because: "Required value for 'Id' is missing.");
    }

    [Given(@"the user created multiple Tokens")]
    public async Task GivenTheUserCreatedMultipleTokens()
    {
        for (var i = 0; i < TOKEN_ITERATIONS; i++)
        {
            var requestConfiguration = new RequestConfiguration();
            requestConfiguration.SupplementWith(_requestConfiguration);
            requestConfiguration.Content = "QQ==";
            requestConfiguration.ExpiresAt = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
            var response = await _tokensApi.CreateToken(requestConfiguration);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            _createdTokens.Add(response.Data!.Result!);
        }
    }

    [When(@"a GET request is sent to the Tokens endpoint with the Tokens list")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithTheTokensList()
    {
        var tokenIds = _createdTokens.Select(t => t.Id);

        var response = await _tokensApi.GetTokenById(_requestConfiguration, tokenIds);

        _tokenResponse.StatusCode = response.StatusCode;

        var tokens = response.Data!.Result!;
        tokens.Should().NotBeNull();
        tokens.Should().HaveCount(_createdTokens.Count);

        _responseTokens.AddRange(tokens);
    }

    [When(@"a POST request is sent to the Tokens endpoint with")]
    public async Task WhenAPOSTRequestIsSentToTheTokensEndpointWith(Table table)
    {
        var requestConfiguration = table.CreateInstance<RequestConfiguration>();
        requestConfiguration.SupplementWith(_requestConfiguration);

        switch (requestConfiguration.ExpiresAt)
        {
            case "<tomorrow>":
                requestConfiguration.ExpiresAt = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
                break;
            case "<yesterday>":
                requestConfiguration.ExpiresAt = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                break;
            default:
                requestConfiguration.ExpiresAt = DateTime.Now.ToString("dd-MM-yyyy");
                break;
        }

        _tokenResponse = await _tokensApi.CreateToken(requestConfiguration);
    }

    [When(@"a POST request is sent to the Tokens endpoint with no request content")]
    public async Task WhenAPOSTRequestIsSentToTheTokensEndpointWithNoRequestContent()
    {
        _requestConfiguration.Content = null;
        _requestConfiguration.ExpiresAt = null;
        _tokenResponse = await _tokensApi.CreateToken(_requestConfiguration);
    }

    [When(@"a GET request is sent to the Tokens/{id} endpoint with ""?(.*?)""?")]
    public async Task WhenAGETRequestIsSentToTheTokensIdEndpointWith(string id)
    {
        switch (id)
        {
            case "t.Id":
                id = _tokenId!;
                break;
            case "p.Id":
                id = _peerTokenId!;
                break;
            case "a valid Id":
                id = "TOKjVPS6h1082AuBVBaR";
                break;
        }
        _tokenResponse = await _tokensApi.GetTokenById(_requestConfiguration, id);
    }

    [When(@"a POST request is sent to the Tokens endpoint with '([^']*)', '([^']*)'")]
    public async Task WhenAPOSTRequestIsSentToTheTokensEndpointWith(string content, string expiresAt)
    {
        var requestConfiguration = new RequestConfiguration
        {
            Content = content,
            ExpiresAt = expiresAt
        };

        requestConfiguration.SupplementWith(_requestConfiguration);

        var httpResponse = await _tokensApi.CreateToken(requestConfiguration);
        _tokenResponse = httpResponse;
    }

    [Then(@"the response does not contain any peer Tokens")]
    public void ThenTheResponseDoesNotContainAnyPeerTokens()
    {
        var ownToken = _createdTokens.First();

        _responseTokens.Should().NotBeEmpty();

        var ownTokenFromResponse = _responseTokens.FirstOrDefault(x => x.Id == ownToken.Id);
        ownTokenFromResponse.Should().NotBeNull();

        _responseTokens.All(token => token.CreatedBy == ownTokenFromResponse!.CreatedBy).Should().BeTrue();
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _tokenResponse.Data!.Error.Should().NotBeNull();

        _tokenResponse.Data!.Error!.Code.Should().Be(errorCode);
    }

    [Then(@"the response contains a Token")]
    public void ThenTheResponseContainsAToken()
    {
        _tokenResponse.Should().NotBeNull();

        AssertStatusCodeIsSuccess();

        AssertResponseContentTypeIsJson();

        AssertResponseContentCompliesWithSchema();

        if (_tokenResponse.HttpMethod == Method.Get.ToString())
        {
            AssertExpirationDateIsInFuture();
        }
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_tokenResponse.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    private void AssertStatusCodeIsSuccess()
    {
        _tokenResponse.IsSuccessStatusCode.Should().BeTrue();
    }

    private void AssertResponseContentTypeIsJson()
    {
        _tokenResponse.ContentType.Should().Be("application/json");
    }

    private void AssertResponseContentCompliesWithSchema()
    {
        JsonValidators.ValidateJsonSchema<TokenResponse<Token>>(_tokenResponse.Content!, out var errors)
            .Should().BeTrue($"Response content does not comply with the {nameof(TokenResponse<Token>)} schema: {string.Join(", ", errors)}");
    }

    private void AssertExpirationDateIsInFuture()
    {
        _tokenResponse.Data!.Result!.ExpiresAt.Should().BeAfter(DateTime.Now);
    }
}

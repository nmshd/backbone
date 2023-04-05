using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
    private string _tokenId;
    private string _peerTokenId;
    private readonly List<Token> _givenOwnTokens;
    private readonly List<Token> _responseTokens;
    private HttpResponse<Response<Token>> _tokenResponse;
    private readonly RequestConfiguration _requestConfiguration;

    public TokensApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, TokensApi tokensApi)
    {
        _tokensApi = tokensApi;
        _tokenId = string.Empty;
        _peerTokenId = string.Empty;
        _givenOwnTokens = new List<Token>();
        _responseTokens = new List<Token>();
        _tokenResponse = new HttpResponse<Response<Token>>();
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
        var createTokenRequest = new CreateTokenRequest
        {
            Content = "QQ==",
            ExpiresAt = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy")
        };

        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.Content = JsonConvert.SerializeObject(createTokenRequest);

        var httpResponse = await _tokensApi.CreateToken(requestConfiguration);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();

        var token = httpResponse.Data!.Result!;

        _tokenId = token.Id;
        _tokenId.Should().NotBeNullOrEmpty(because: "Required value for 'Id' is missing.");
    }

    [Given(@"a peer Token p")]
    public async Task GivenAPeerTokenP()
    {
        var createTokenRequest = new CreateTokenRequest
        {
            Content = "QQ==",
            ExpiresAt = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy")
        };

        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.AuthenticationParameters.Username = "USRb";
        requestConfiguration.AuthenticationParameters.Password = "b";
        requestConfiguration.Content = JsonConvert.SerializeObject(createTokenRequest);

        var httpResponse = await _tokensApi.CreateToken(requestConfiguration);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();

        var token = httpResponse.Data!.Result!;

        _peerTokenId = token.Id!;
        _peerTokenId.Should().NotBeNullOrEmpty(because: "Required value for 'Id' is missing.");
    }

    [Given(@"the user created multiple Tokens")]
    public async Task GivenTheUserCreatedMultipleTokens()
    {
        for (var i = 0; i < 2; i++)
        {
            var createTokenRequest = new CreateTokenRequest
            {
                Content = "QQ==",
                ExpiresAt = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy")
            };

            var requestConfiguration = new RequestConfiguration
            {
                Content = JsonConvert.SerializeObject(createTokenRequest)
            };
            requestConfiguration.SupplementWith(_requestConfiguration);
            var response = await _tokensApi.CreateToken(requestConfiguration);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            _givenOwnTokens.Add(response.Data!.Result!);
        }
    }

    [When(@"a GET request is sent to the Tokens endpoint with a list of ids of own Tokens")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithAListOfIdsOfOwnTokens()
    {
        var tokenIds = _givenOwnTokens.Select(t => t.Id);

        var response = await _tokensApi.GetTokenById(_requestConfiguration, tokenIds);

        _tokenResponse.StatusCode = response.StatusCode;

        var tokens = response.Data!.Result!;
        tokens.Should().NotBeNull();
        tokens.Should().HaveCount(_givenOwnTokens.Count);

        _responseTokens.AddRange(tokens);
    }

    [When(@"a POST request is sent to the Tokens endpoint with")]
    public async Task WhenAPOSTRequestIsSentToTheTokensEndpointWith(Table table)
    {
        var requestConfiguration = table.CreateInstance<RequestConfiguration>();
        requestConfiguration.SupplementWith(_requestConfiguration);

        if (!string.IsNullOrEmpty(requestConfiguration.Content))
        {
            switch (requestConfiguration.Content)
            {
                case var c when c.Contains("<tomorrow>"):
                    requestConfiguration.Content = requestConfiguration.Content.Replace("<tomorrow>", DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"));
                    break;
                case var c when c.Contains("<yesterday>"):
                    requestConfiguration.Content = requestConfiguration.Content.Replace("<yesterday>", DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy"));
                    break;
                default:
                    break;
            }
        }

        _tokenResponse = await _tokensApi.CreateToken(requestConfiguration);
    }

    [When(@"a POST request is sent to the Tokens endpoint with no request content")]
    public async Task WhenAPOSTRequestIsSentToTheTokensEndpointWithNoRequestContent()
    {
        _requestConfiguration.Content = null;
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
        var createTokenRequest = new CreateTokenRequest
        {
            Content = content,
            ExpiresAt = expiresAt
        };

        var requestConfiguration = new RequestConfiguration
        {
            Content = JsonConvert.SerializeObject(createTokenRequest)
        };

        requestConfiguration.SupplementWith(_requestConfiguration);

        var httpResponse = await _tokensApi.CreateToken(requestConfiguration);
        _tokenResponse = httpResponse;
    }

    [Then(@"the response contains all Tokens with the given ids")]
    public void ThenTheResponseContainsAllTokensWithTheGivenIds()
    {
        _responseTokens.Select(t => t.Id)
            .Should()
            .HaveCount(_givenOwnTokens.Count)
            .And.BeEquivalentTo(_givenOwnTokens.Select(t => t.Id), options => options.WithoutStrictOrdering());
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

        if (_tokenResponse.HttpMethod == Method.Get.ToString())
        {
            AssertResponseContentCompliesWithSchema<Token>();
        }
        else if (_tokenResponse.HttpMethod == Method.Post.ToString())
        {
            AssertResponseContentCompliesWithSchema<CreateTokenResponse>();
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

    private void AssertResponseContentCompliesWithSchema<T>()
    {
        JsonValidators.ValidateJsonSchema<Response<T>>(_tokenResponse.Content!, out var errors)
            .Should().BeTrue($"Response content does not comply with the {nameof(Response<T>)} schema: {string.Join(", ", errors)}");
    }
}

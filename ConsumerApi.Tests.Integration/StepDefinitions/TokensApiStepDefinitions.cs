using System.Net;
using ConsumerApi.Tests.Integration.API;
using ConsumerApi.Tests.Integration.Configuration;
using ConsumerApi.Tests.Integration.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using TechTalk.SpecFlow.Assist;

namespace ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Token")]
[Scope(Feature = "GET Token")]
public class TokensApiStepDefinitions : BaseStepDefinitions<Token>
{
    private readonly TokensApi _tokensApi;
    private string _tokenId;
    private string _peerTokenId;
    private readonly List<Token> _givenOwnTokens;
    private readonly List<Token> _responseTokens;
    private static readonly string TomorrowAsString = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
    private static readonly string YesterdayAsString = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

    public TokensApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, TokensApi tokensApi) : base(httpConfiguration, new HttpResponse<Token>())
    {
        _tokensApi = tokensApi;
        _tokenId = string.Empty;
        _peerTokenId = string.Empty;
        _givenOwnTokens = new List<Token>();
        _responseTokens = new List<Token>();
    }

    [Given(@"an own Token t")]
    public async Task GivenAnOwnTokenT()
    {
        var createTokenRequest = new CreateTokenRequest
        {
            Content = "QQ==",
            ExpiresAt = TomorrowAsString
        };

        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.Content = JsonConvert.SerializeObject(createTokenRequest);

        var httpResponse = await _tokensApi.CreateToken(requestConfiguration);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();

        var token = httpResponse.Content!.Result!;

        _tokenId = token.Id;
        _tokenId.Should().NotBeNullOrEmpty();
    }

    [Given(@"a peer Token p")]
    public async Task GivenAPeerTokenP()
    {
        var createTokenRequest = new CreateTokenRequest
        {
            Content = "QQ==",
            ExpiresAt = TomorrowAsString
        };

        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.AuthenticationParameters.Username = "USRb";
        requestConfiguration.AuthenticationParameters.Password = "b";
        requestConfiguration.Content = JsonConvert.SerializeObject(createTokenRequest);

        var httpResponse = await _tokensApi.CreateToken(requestConfiguration);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();

        var token = httpResponse.Content!.Result!;

        _peerTokenId = token.Id!;
        _peerTokenId.Should().NotBeNullOrEmpty();
    }

    [Given(@"the user created multiple Tokens")]
    public async Task GivenTheUserCreatedMultipleTokens()
    {
        for (var i = 0; i < 2; i++)
        {
            var createTokenRequest = new CreateTokenRequest
            {
                Content = "QQ==",
                ExpiresAt = TomorrowAsString
            };

            var requestConfiguration = new RequestConfiguration
            {
                Content = JsonConvert.SerializeObject(createTokenRequest)
            };
            requestConfiguration.SupplementWith(_requestConfiguration);
            var response = await _tokensApi.CreateToken(requestConfiguration);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            _givenOwnTokens.Add(response.Content!.Result!);
        }
    }

    [When(@"a GET request is sent to the Tokens endpoint with a list of ids of own Tokens")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithAListOfIdsOfOwnTokens()
    {
        var tokenIds = _givenOwnTokens.Select(t => t.Id);

        var response = await _tokensApi.GetTokensByIds(_requestConfiguration, tokenIds);

        var tokens = response.Content!.Result!;
        tokens.Should().NotBeNull();
        tokens.Should().HaveCount(_givenOwnTokens.Count);

        _response.StatusCode = response.StatusCode;
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
                    requestConfiguration.Content = requestConfiguration.Content.Replace("<tomorrow>", TomorrowAsString);
                    break;
                case var c when c.Contains("<yesterday>"):
                    requestConfiguration.Content = requestConfiguration.Content.Replace("<yesterday>", YesterdayAsString);
                    break;
                default:
                    break;
            }
        }

        _response = await _tokensApi.CreateToken(requestConfiguration);
    }

    [When(@"a POST request is sent to the Tokens endpoint with no request content")]
    public async Task WhenAPOSTRequestIsSentToTheTokensEndpointWithNoRequestContent()
    {
        _requestConfiguration.Content = null;
        _response = await _tokensApi.CreateToken(_requestConfiguration);
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
        _response = await _tokensApi.GetTokenById(_requestConfiguration, id);
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

        _response = await _tokensApi.CreateToken(requestConfiguration);
    }

    [When(@"a GET request is sent to the Tokens endpoint with a list containing t\.Id, p\.Id")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithAListContainingT_IdP_Id()
    {
        var tokenIds = new List<string> { _tokenId, _peerTokenId };
        var response = await _tokensApi.GetTokensByIds(_requestConfiguration, tokenIds);

        _response.StatusCode = response.StatusCode;

        var tokens = response.Content!.Result!;

        _responseTokens.AddRange(tokens);
    }

    [Then(@"the response contains both Tokens")]
    public void ThenTheResponseOnlyContainsTheOwnToken()
    {
        _responseTokens.Should().HaveCount(2)
            .And.Contain(token => token.Id == _tokenId)
            .And.Contain(token => token.Id == _peerTokenId);
    }


    [Then(@"the response contains all Tokens with the given ids")]
    public void ThenTheResponseContainsAllTokensWithTheGivenIds()
    {
        _responseTokens.Select(t => t.Id)
            .Should()
            .HaveCount(_givenOwnTokens.Count)
            .And.BeEquivalentTo(_givenOwnTokens.Select(t => t.Id), options => options.WithoutStrictOrdering());
    }

    [Then(@"the response contains a Token")]
    public void ThenTheResponseContainsAToken()
    {
        AssertResponseIntegrity();
        if (_response.HttpMethod == Method.Get.ToString())
        {
            AssertResponseContentCompliesWithSchema<Token>();
        }
        else if (_response.HttpMethod == Method.Post.ToString())
        {
            AssertResponseContentCompliesWithSchema<CreateTokenResponse>();
        }
    }
}

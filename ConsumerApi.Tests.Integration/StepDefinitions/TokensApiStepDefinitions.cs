using System.Net;
using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Assist;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Token")]
[Scope(Feature = "GET Token")]
[Scope(Feature = "GET Tokens")]
internal class TokensApiStepDefinitions : BaseStepDefinitions
{
    private readonly TokensApi _tokensApi;
    private string _tokenId;
    private string _peerTokenId;
    private readonly List<CreateTokenResponse> _givenOwnTokens;
    private readonly List<Token> _responseTokens;
    private HttpResponse<CreateTokenResponse>? _createTokenResponse;
    private HttpResponse<Token>? _tokenResponse;
    private HttpResponse<IEnumerable<Token>>? _tokensResponse;
    private static readonly string TOMORROW_AS_STRING = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
    private static readonly string YESTERDAY_AS_STRING = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

    public TokensApiStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, TokensApi tokensApi, ISignatureHelper signatureHelper, ChallengesApi challengesApi, IdentitiesApi identitiesApi, DevicesApi devicesApi) :
        base(httpConfiguration, signatureHelper, challengesApi, identitiesApi, devicesApi)
    {
        _tokensApi = tokensApi;
        _tokenId = string.Empty;
        _peerTokenId = string.Empty;
        _givenOwnTokens = [];
        _responseTokens = [];
    }

    [Given(@"an own Token t")]
    public async Task GivenAnOwnTokenT()
    {
        var createTokenRequest = new CreateTokenRequest
        {
            Content = "QQ==",
            ExpiresAt = TOMORROW_AS_STRING
        };

        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.Content = JsonConvert.SerializeObject(createTokenRequest);
        requestConfiguration.ContentType = "application/json";

        var response = await _tokensApi.CreateToken(requestConfiguration);
        response.IsSuccessStatusCode.Should().BeTrue();

        var token = response.Content.Result!;
        _tokenId = token.Id;
        _tokenId.Should().NotBeNullOrEmpty();
    }

    [Given(@"a peer Token p")]
    public async Task GivenAPeerTokenP()
    {
        var createTokenRequest = new CreateTokenRequest
        {
            Content = "QQ==",
            ExpiresAt = TOMORROW_AS_STRING
        };

        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.Authenticate = true;
        requestConfiguration.AuthenticationParameters.Username = "USRb";
        requestConfiguration.AuthenticationParameters.Password = "b";
        requestConfiguration.Content = JsonConvert.SerializeObject(createTokenRequest);
        requestConfiguration.ContentType = "application/json";

        var response = await _tokensApi.CreateToken(requestConfiguration);
        response.IsSuccessStatusCode.Should().BeTrue();

        var token = response.Content.Result!;
        _peerTokenId = token.Id;
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
                ExpiresAt = TOMORROW_AS_STRING
            };

            var requestConfiguration = new RequestConfiguration
            {
                Content = JsonConvert.SerializeObject(createTokenRequest),
                ContentType = "application/json"
            };
            requestConfiguration.SupplementWith(_requestConfiguration);
            var response = await _tokensApi.CreateToken(requestConfiguration);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            _givenOwnTokens.Add(response.Content.Result!);
        }
    }

    [When(@"a GET request is sent to the Tokens endpoint with a list of ids of own Tokens")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithAListOfIdsOfOwnTokens()
    {
        var tokenIds = _givenOwnTokens.Select(t => t.Id);

        _tokensResponse = await _tokensApi.GetTokensByIds(_requestConfiguration, tokenIds);
        _tokensResponse.Should().NotBeNull();

        var tokens = _tokensResponse.Content.Result!.ToArray();
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
                    requestConfiguration.Content = requestConfiguration.Content.Replace("<tomorrow>", TOMORROW_AS_STRING);
                    break;
                case var c when c.Contains("<yesterday>"):
                    requestConfiguration.Content = requestConfiguration.Content.Replace("<yesterday>", YESTERDAY_AS_STRING);
                    break;
            }
        }

        _createTokenResponse = await _tokensApi.CreateToken(requestConfiguration);
    }

    [When(@"a POST request is sent to the Tokens endpoint with no request content")]
    public async Task WhenAPOSTRequestIsSentToTheTokensEndpointWithNoRequestContent()
    {
        _requestConfiguration.Content = null;
        _createTokenResponse = await _tokensApi.CreateToken(_requestConfiguration);
    }

    [When(@"a GET request is sent to the Tokens/{id} endpoint with ""?(.*?)""?")]
    public async Task WhenAGETRequestIsSentToTheTokensIdEndpointWith(string id)
    {
        switch (id)
        {
            case "t.Id":
                id = _tokenId;
                break;
            case "p.Id":
                id = _peerTokenId;
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
            Content = JsonConvert.SerializeObject(createTokenRequest),
            ContentType = "application/json"
        };

        requestConfiguration.SupplementWith(_requestConfiguration);

        _createTokenResponse = await _tokensApi.CreateToken(requestConfiguration);
    }

    [When(@"a GET request is sent to the Tokens endpoint with a list containing t\.Id, p\.Id")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithAListContainingT_IdP_Id()
    {
        var tokenIds = new List<string> { _tokenId, _peerTokenId };
        _tokensResponse = await _tokensApi.GetTokensByIds(_requestConfiguration, tokenIds);

        var tokens = _tokensResponse.Content.Result!;
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

    [Then(@"the response contains a CreateTokenResponse")]
    public void ThenTheResponseContainsACreateTokenResponse()
    {
        _createTokenResponse!.Should().NotBeNull();
        _createTokenResponse!.IsSuccessStatusCode.Should().BeTrue();
        _createTokenResponse!.ContentType.Should().Be("application/json");
        _createTokenResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response contains a Token")]
    public void ThenTheResponseContainsAToken()
    {
        _tokenResponse!.Should().NotBeNull();
        _tokenResponse!.IsSuccessStatusCode.Should().BeTrue();
        _tokenResponse!.ContentType.Should().Be("application/json");
        _tokenResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_tokensResponse != null)
        {
            var actualStatusCode = (int)_tokensResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_tokenResponse != null)
        {
            var actualStatusCode = (int)_tokenResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_createTokenResponse != null)
        {
            var actualStatusCode = (int)_createTokenResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _tokenResponse!.Content.Error.Should().NotBeNull();
        _tokenResponse.Content.Error!.Code.Should().Be(errorCode);
    }
}

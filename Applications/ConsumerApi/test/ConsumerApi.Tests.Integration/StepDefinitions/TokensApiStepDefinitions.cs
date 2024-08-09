using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Backbone.Crypto;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Token")]
[Scope(Feature = "GET Token")]
[Scope(Feature = "GET Tokens")]
internal class TokensApiStepDefinitions : BaseStepDefinitions
{
    private string _tokenId;
    private string _peerTokenId;
    private readonly List<CreateTokenResponse> _givenOwnTokens;
    private readonly List<Token> _responseTokens;
    private ApiResponse<CreateTokenResponse>? _createTokenResponse;
    private ApiResponse<EmptyResponse>? _createTokenResponse401;
    private ApiResponse<Token>? _tokenResponse;
    private ApiResponse<ListTokensResponse>? _tokensResponse;
    private bool _isAuthenticated;
    private Client? _sdk;

    private static readonly DateTime TOMORROW = DateTime.Now.AddDays(1);

    private static readonly byte[] CONTENT = JsonConvert.SerializeObject(new { key = "some-value" }).GetBytes();

    public TokensApiStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration) : base(factory, httpConfiguration)
    {
        _isAuthenticated = false;
        _tokenId = string.Empty;
        _peerTokenId = string.Empty;
        _givenOwnTokens = [];
        _responseTokens = [];
    }

    [Given("the user is authenticated")]
    public async Task GivenTheUserIsAuthenticated()
    {
        _sdk = await Client.CreateForNewIdentity(HttpClient, ClientCredentials, Constants.DEVICE_PASSWORD);
        _isAuthenticated = true;
    }

    [Given("the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _sdk = Client.CreateUnauthenticated(HttpClient, ClientCredentials);
        _isAuthenticated = false;
    }

    [Given("an own Token t")]
    public async Task GivenAnOwnTokenT()
    {
        var createTokenRequest = new CreateTokenRequest
        {
            Content = CONTENT,
            ExpiresAt = TOMORROW
        };

        _sdk = await Client.CreateForNewIdentity(HttpClient, ClientCredentials, Constants.DEVICE_PASSWORD);
        var response = await _sdk.Tokens.CreateToken(createTokenRequest);
        response.Should().BeASuccess();

        _tokenId = response.Result!.Id;
        _tokenId.Should().NotBeNullOrEmpty();
    }

    [Given("a peer Token p")]
    public async Task GivenAPeerTokenP()
    {
        var createTokenRequest = new CreateTokenRequest
        {
            Content = CONTENT,
            ExpiresAt = TOMORROW
        };

        var response = await _sdk!.Tokens.CreateToken(createTokenRequest);
        response.Should().BeASuccess();

        _peerTokenId = response.Result!.Id;
        _peerTokenId.Should().NotBeNullOrEmpty();
    }

    [Given("the user created multiple Tokens")]
    public async Task GivenTheUserCreatedMultipleTokens()
    {
        for (var i = 0; i < 2; i++)
        {
            var createTokenRequest = new CreateTokenRequest
            {
                Content = CONTENT,
                ExpiresAt = TOMORROW
            };

            var response = await _sdk!.Tokens.CreateToken(createTokenRequest);

            response.Should().BeASuccess();

            _givenOwnTokens.Add(response.Result!);
        }
    }

    [Given("a Token t created by (i[a-zA-Z0-9]*) where forIdentity is the address of (i[a-zA-Z0-9]*)")]
    public async Task GivenATokenTCreatedByIWhereForIdentityIsTheAddressOfI(string createdByIdentity, string createdForIdentity)
    {
        var response = await Identities[createdByIdentity].Tokens.CreateToken(new CreateTokenRequest()
        {
            Content = CONTENT,
            ExpiresAt = TOMORROW,
            ForIdentity = Identities[createdForIdentity].IdentityData!.Address
        });

        response.Should().BeASuccess();

        _tokenId = response.Result!.Id;
    }

    [When("(i[a-zA-Z0-9]*) sends a GET request to the /Tokens/{id} endpoint with t.id")]
    public async Task WhenISendsAGETRequestToTheTokensIdEndpointWithT_Id(string identity)
    {
        _tokenResponse = await Identities[identity].Tokens.GetToken(_tokenId);
    }


    [When("a GET request is sent to the Tokens endpoint with a list of ids of own Tokens")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithAListOfIdsOfOwnTokens()
    {
        var tokenIds = _givenOwnTokens.Select(t => t.Id);

        _tokensResponse = await _sdk!.Tokens.ListTokens(tokenIds);
        _tokensResponse.Should().NotBeNull();

        var tokens = _tokensResponse.Result!.ToArray();
        tokens.Should().HaveCount(_givenOwnTokens.Count);

        _responseTokens.AddRange(tokens);
    }

    [When("a POST request is sent to the Tokens endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheTokensEndpointWith()
    {
        var request = new CreateTokenRequest
        {
            Content = CONTENT,
            ExpiresAt = TOMORROW
        };

        if (_isAuthenticated)
        {
            _createTokenResponse = await _sdk!.Tokens.CreateToken(request);
        }
        else
        {
            _createTokenResponse401 = await _sdk!.Tokens.CreateTokenUnauthenticated(request);
        }
    }

    [When("a POST request is sent to the Tokens endpoint with invalid Content Type")]
    public async Task WhenAPOSTRequestIsSentToTheTokensEndpointWithNoRequestContent()
    {
        var request = new CreateTokenRequest
        {
            Content = CONTENT,
            ExpiresAt = TOMORROW
        };

        _createTokenResponse = await _sdk!.Tokens.CreateToken(request);
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

        _tokenResponse = await _sdk!.Tokens.GetTokenUnauthenticated(id);
    }

    [When(@"a GET request is sent to the Tokens endpoint with a list containing t\.Id, p\.Id")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithAListContainingT_IdP_Id()
    {
        var tokenIds = new List<string> { _tokenId, _peerTokenId };
        _tokensResponse = await _sdk!.Tokens.ListTokens(tokenIds);

        _responseTokens.AddRange(_tokensResponse.Result!);
    }

    [When("(i[a-zA-Z0-9]*) sends a GET request to the /Tokens endpoint and passes t.id")]
    public async Task WhenISendsAGETRequestToTheTokensEndpointAndPassesT_IdAsync(string identity)
    {
        var tokenIds = new List<string> { _tokenId };
        _tokensResponse = await Identities[identity].Tokens.ListTokens(tokenIds);

        _responseTokens.AddRange(_tokensResponse.Result!);
    }


    [Then("the response contains both Tokens")]
    public void ThenTheResponseOnlyContainsTheOwnToken()
    {
        _responseTokens.Should().HaveCount(2)
            .And.Contain(token => token.Id == _tokenId)
            .And.Contain(token => token.Id == _peerTokenId);
    }

    [Then("the response contains all Tokens with the given ids")]
    public void ThenTheResponseContainsAllTokensWithTheGivenIds()
    {
        _responseTokens.Select(t => t.Id)
            .Should()
            .HaveCount(_givenOwnTokens.Count)
            .And.BeEquivalentTo(_givenOwnTokens.Select(t => t.Id), options => options.WithoutStrictOrdering());
    }

    [Then("the response contains a CreateTokenResponse")]
    public async Task ThenTheResponseContainsACreateTokenResponse()
    {
        _createTokenResponse!.Should().NotBeNull();
        _createTokenResponse!.Should().BeASuccess();
        _createTokenResponse!.ContentType.Should().Be("application/json");
        await _createTokenResponse!.Should().ComplyWithSchema();
    }

    [Then("the response contains a Token")]
    public async Task ThenTheResponseContainsAToken()
    {
        _tokenResponse!.Should().NotBeNull();
        _tokenResponse!.Should().BeASuccess();
        _tokenResponse!.ContentType.Should().Be("application/json");
        await _tokenResponse!.Should().ComplyWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_tokensResponse != null)
            ((int)_tokensResponse.Status).Should().Be(expectedStatusCode);

        if (_tokenResponse != null)
            ((int)_tokenResponse.Status).Should().Be(expectedStatusCode);

        if (_createTokenResponse != null)
            ((int)_createTokenResponse.Status).Should().Be(expectedStatusCode);

        if (_createTokenResponse401 != null)
            ((int)_createTokenResponse401.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _tokenResponse!.Error.Should().NotBeNull();
        _tokenResponse.Error!.Code.Should().Be(errorCode);
    }

    [Then("the response contains t")]
    public void ThenTheResponseContainsT()
    {
        if (_tokenResponse is not null)
        {
            _tokenResponse.Should().BeASuccess();
            _tokenResponse.Result!.Id.Should().Be(_tokenId);
        }
        if (_tokensResponse is not null)
        {
            _tokensResponse.Should().BeASuccess();
            _tokensResponse.Result!.Select(x => x.Id).Should().Contain(_tokenId);
        }
    }

    [Then("the response does not contain t")]
    public void ThenTheResponseDoesNotContainT()
    {
        if (_tokensResponse is not null)
        {
            _tokensResponse.Should().BeASuccess();
            _tokensResponse.Result!.Select(x => x.Id).Should().NotContain(_tokenId);
        }
    }
}

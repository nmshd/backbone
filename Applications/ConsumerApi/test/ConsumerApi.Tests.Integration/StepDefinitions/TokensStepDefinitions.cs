using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.Crypto;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class TokensStepDefinitions
{
    #region Constructor, Fields, Properties
    private static readonly DateTime TOMORROW = DateTime.Now.AddDays(1);

    private static readonly byte[] CONTENT = ConvertibleString.FromUtf8(JsonConvert.SerializeObject(new
    {
        key = "some-value"
    })).BytesRepresentation;

    private readonly HttpClient _httpClient;
    private readonly ClientCredentials _clientCredentials;

    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;
    private readonly TokensContext _tokensContext;

    public TokensStepDefinitions(IdentitiesContext identitiesContext, ResponseContext responseContext, TokensContext tokensContext, HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
        _tokensContext = tokensContext;
    }

    private ClientPool ClientPool => _identitiesContext.ClientPool;

    private CreateTokenRequest CreateTokenRequest => new() { Content = CONTENT, ExpiresAt = TOMORROW };
    #endregion

    #region Given
    [Given("([a-zA-Z0-9]+) created multiple Tokens")]
    public async Task GivenTheIdentityCreatedMultipleTokens(string identityName)
    {
        for (var i = 0; i < 2; i++)
        {
            var client = ClientPool.FirstForIdentity(identityName)!;

            var response = await client.Tokens.CreateToken(CreateTokenRequest);
            response.Should().BeASuccess();

            _tokensContext.AddCreateTokenResponse(client.IdentityData!.Address, $"t{i}", response.Result!);
        }
    }

    [Given(@"Identity ([a-zA-Z0-9]+) and Token ([a-zA-Z0-9]+)")]
    public async Task GivenIdentityAndToken(string identityName, string tokenName)
    {
        var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);
        ClientPool.Add(client).ForIdentity(identityName);

        var response = await client.Tokens.CreateToken(CreateTokenRequest);
        response.Should().BeASuccess();
        response.Result!.Id.Should().NotBeNullOrEmpty();

        _tokensContext.AddCreateTokenResponse(client.IdentityData!.Address, tokenName, response.Result!);
    }
    #endregion

    #region When
    [When("([a-zA-Z0-9]+) sends a GET request to the Tokens endpoint with a list of ids of own Tokens")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensEndpointWithAListOfIdsOfOwnTokens(string identityName)
    {
        var client = ClientPool.FirstForIdentity(identityName)!;
        var tokenIds = _tokensContext.CreateTokenResponses.Values.Where(t => t.CreatedBy == client.IdentityData!.Address).Select(t => t.CreateTokenResponse.Id);

        _responseContext.WhenResponse = _responseContext.ListTokensResponse = await client.Tokens.ListTokens(tokenIds);
        _responseContext.WhenResponse.Should().NotBeNull();

        var tokensOfIdentityCount = _tokensContext.CreateTokenResponses.Values.Count(t => t.CreatedBy == client.IdentityData!.Address);
        var tokens = _responseContext.ListTokensResponse.Result!.ToArray();
        tokens.Should().HaveCount(tokensOfIdentityCount);

        _responseContext.ResponseTokens.AddRange(tokens);
    }

    [When(@"([a-zA-Z0-9]+) sends a GET request to the Tokens endpoint with a list containing ([a-zA-Z0-9]+).Id, ([a-zA-Z0-9]+).Id")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensEndpointWithAListContainingTokenIds(string identityName, string tokenName, string peerTokenName)
    {
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].CreateTokenResponse.Id;
        var peerTokenId = _tokensContext.CreateTokenResponses[peerTokenName].CreateTokenResponse.Id;

        var tokenIds = new List<string> { tokenId, peerTokenId };
        _responseContext.WhenResponse = _responseContext.ListTokensResponse = await ClientPool.FirstForIdentity(identityName)!.Tokens.ListTokens(tokenIds);

        _responseContext.ResponseTokens.AddRange(_responseContext.ListTokensResponse.Result!);
    }


    [When(@"([a-zA-Z0-9]+) sends a POST request to the Tokens endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheTokensEndpoint(string identityName)
    {
        _responseContext.WhenResponse = _responseContext.CreateTokenResponse = await ClientPool.FirstForIdentity(identityName)!.Tokens.CreateToken(CreateTokenRequest);
    }

    [When("a POST request is sent to the Tokens endpoint")]
    public async Task WhenAPostRequestIsSentToTheTokensEndpointWith()
    {
        _responseContext.WhenResponse = _responseContext.CreateTokenAnonymously = await ClientPool.Default()!.Tokens.CreateTokenUnauthenticated(CreateTokenRequest);
    }

    [When(@"([a-zA-Z0-9]+) sends a GET request to the Tokens/\{id} endpoint with ([a-zA-Z0-9]+).Id")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensIdEndpointWithTokenId(string identityName, string tokenName)
    {
        var client = ClientPool.FirstForIdentity(identityName)!;
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].CreateTokenResponse.Id;

        _responseContext.WhenResponse = _responseContext.GetTokenResponse = await client.Tokens.GetToken(tokenId);
    }

    [When(@"a GET request is sent to the Tokens/{id} endpoint with ([a-zA-Z0-9]+).Id")]
    public async Task WhenAGetRequestIsSentToTheTokensIdEndpointWithTokenId(string tokenName)
    {
        var client = ClientPool.Anonymous!;
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].CreateTokenResponse.Id;

        _responseContext.WhenResponse = _responseContext.GetTokenResponse = await client.Tokens.GetToken(tokenId);
    }

    [When(@"([a-zA-Z0-9]+) sends a GET request to the Tokens/{id} endpoint with ""([^""]*)""")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensIdEndpointWithNonExistingTokenId(string identityName, string nonExistingTokenId)
    {
        var client = ClientPool.FirstForIdentity(identityName)!;
        _responseContext.WhenResponse = _responseContext.GetTokenResponse = await client.Tokens.GetToken(nonExistingTokenId);
    }
    #endregion

    #region Then
    [Then("the response contains all Tokens created by ([a-zA-Z0-9]+) with the given ids")]
    public void ThenTheResponseContainsAllTokensCreatedByIdentityWithTheGivenIds(string identityName)
    {
        var client = ClientPool.FirstForIdentity(identityName)!;
        var tokenIds = _tokensContext.CreateTokenResponses.Values.Where(t => t.CreatedBy == client.IdentityData!.Address).Select(t => t.CreateTokenResponse).ToList();

        _responseContext.ResponseTokens.Select(t => t.Id)
            .Should()
            .HaveCount(tokenIds.Count)
            .And.BeEquivalentTo(tokenIds.Select(t => t.Id), options => options.WithoutStrictOrdering());
    }

    [Then(@"the response contains ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public void ThenTheResponseContainsBothTokens(string tokenName, string peerTokenName)
    {
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].CreateTokenResponse.Id;
        var peerTokenId = _tokensContext.CreateTokenResponses[peerTokenName].CreateTokenResponse.Id;

        _responseContext.ResponseTokens.Should().HaveCount(2)
            .And.Contain(token => token.Id == tokenId)
            .And.Contain(token => token.Id == peerTokenId);
    }
    #endregion
}

public class TokensContext
{
    public Dictionary<string, CreateTokenResponseWrapper> CreateTokenResponses = new();

    public void AddCreateTokenResponse(string createdBy, string tokenName, CreateTokenResponse createTokenResponse)
    {
        CreateTokenResponses[tokenName] = new CreateTokenResponseWrapper
        {
            CreatedBy = createdBy,
            CreateTokenResponse = createTokenResponse
        };
    }

    public class CreateTokenResponseWrapper
    {
        public required string CreatedBy;
        public required CreateTokenResponse CreateTokenResponse;
    }
}

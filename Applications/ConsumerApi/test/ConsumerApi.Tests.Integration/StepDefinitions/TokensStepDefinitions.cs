using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class TokensStepDefinitions
{
    #region Constructor, Fields, Properties

    private static readonly DateTime TOMORROW = DateTime.Now.AddDays(1);

    private readonly ResponseContext _responseContext;
    private readonly TokensContext _tokensContext;
    private readonly ClientPool _clientPool;

    private ApiResponse<ListTokensResponse>? _listTokensResponse;

    public TokensStepDefinitions(ResponseContext responseContext, TokensContext tokensContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _tokensContext = tokensContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($"Tokens? {RegexFor.LIST_OF_THINGS} belonging to {RegexFor.SINGLE_THING}")]
    public async Task GivenTheIdentityCreatedMultipleTokens(string tokenNames, string identityName)
    {
        foreach (var tokenName in Utils.SplitNames(tokenNames))
        {
            var client = _clientPool.FirstForIdentityName(identityName);

            var response = await client.Tokens.CreateToken(new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW });
            response.Should().BeASuccess();

            _tokensContext.CreateTokenResponses[tokenName] = response.Result!;
        }
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Tokens endpoint with the ids of {RegexFor.LIST_OF_THINGS}")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensEndpointWithAListOfIdsOfOwnTokens(string identityName, string tokenNames)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var tokenIds = Utils.SplitNames(tokenNames).Select(tokenName => _tokensContext.CreateTokenResponses[tokenName].Id).ToArray();

        _responseContext.WhenResponse = _listTokensResponse = await client.Tokens.ListTokens(tokenIds);
        _responseContext.WhenResponse.Should().NotBeNull();

        var tokens = _listTokensResponse.Result!.ToArray();
        tokens.Should().HaveCount(tokenIds.Length);
    }

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Tokens endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheTokensEndpoint(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = await client.Tokens.CreateToken(new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW });
    }

    [When("an anonymous user sends a POST request to the /Tokens endpoint")]
    public async Task WhenAnAnonymousUserSendsAPOSTRequestIsSentToTheTokensEndpoint()
    {
        _responseContext.WhenResponse = await _clientPool.Anonymous.Tokens.CreateTokenUnauthenticated(new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW });
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Tokens/{{id}} endpoint with {RegexFor.SINGLE_THING}.Id")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensIdEndpointWithTokenId(string identityName, string tokenName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].Id;

        _responseContext.WhenResponse = await client.Tokens.GetToken(tokenId);
    }

    [When($"an anonymous user sends a GET request to the /Tokens/{{id}} endpoint with {RegexFor.SINGLE_THING}.Id")]
    public async Task WhenAnAnonymousUserSendsAGetRequestToTheTokensIdEndpointWithTokenId(string tokenName)
    {
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].Id;
        _responseContext.WhenResponse = await _clientPool.Anonymous.Tokens.GetToken(tokenId);
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Tokens/{{id}} endpoint with \"([^\"]*)\"")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensIdEndpointWithNonExistingTokenId(string identityName, string nonExistingTokenId)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = await client.Tokens.GetToken(nonExistingTokenId);
    }

    #endregion

    #region Then

    [Then($"the response contains the Tokens {RegexFor.SINGLE_THING} and {RegexFor.SINGLE_THING}")]
    public void ThenTheResponseContainsTokens(string token1Name, string token2Name)
    {
        var token1Id = _tokensContext.CreateTokenResponses[token1Name].Id;
        var token2Id = _tokensContext.CreateTokenResponses[token2Name].Id;

        _listTokensResponse!.Result.Should()
            .HaveCount(2).And
            .Contain(token => token.Id == token1Id).And
            .Contain(token => token.Id == token2Id);
    }

    #endregion
}

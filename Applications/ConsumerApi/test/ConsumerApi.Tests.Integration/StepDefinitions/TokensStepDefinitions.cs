using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
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

    [Given($@"Token {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING} with password ""([^""]*)"" and forIdentity {RegexFor.OPTIONAL_SINGLE_THING}")]
    public async Task GivenRelationshipTemplateCreatedByTokenOwnerWithPasswordAndForIdentity(string relationshipTemplateName, string identityName, string passwordString, string forIdentityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var forClient = forIdentityName != "-" ? _clientPool.FirstForIdentityName(forIdentityName).IdentityData!.Address : null;
        var password = passwordString.Trim() != "-" ? Convert.FromBase64String(passwordString.Trim()) : null;

        var response = await client.Tokens.CreateToken(
            new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW, ForIdentity = forClient, Password = password });

        _tokensContext.CreateTokenResponses[relationshipTemplateName] = response.Result!;
    }

    [Given($@"Token {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING}")]
    public async Task GivenTokenCreatedByIdentity(string tokenName, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var response = await client.Tokens.CreateToken(
            new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW, ForIdentity = null, Password = null });

        _tokensContext.CreateTokenResponses[tokenName] = response.Result!;
    }

    [Given(@"the following Tokens")]
    public async Task GivenTheFollowingTokens(Table table)
    {
        var tokenPropertiesSet = table.CreateSet<TokenProperties>();

        foreach (var tokenProperties in tokenPropertiesSet)
        {
            var client = _clientPool.FirstForIdentityName(tokenProperties.TokenOwner);
            var forClient = tokenProperties.ForIdentity != "-" ? _clientPool.FirstForIdentityName(tokenProperties.ForIdentity).IdentityData!.Address : null;
            var password = tokenProperties.Password.Trim() != "-" ? Convert.FromBase64String(tokenProperties.Password.Trim()) : null;
            var allocatedBy = tokenProperties.AllocatedBy.Trim() != "-" ? tokenProperties.AllocatedBy.Split(",").Select(s => s.Trim()).ToList() : [];

            var response = await client.Tokens
                .CreateToken(new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW, ForIdentity = forClient, Password = password });

            _tokensContext.CreateTokenResponses[tokenProperties.TokenName] = response.Result!;

            foreach (var allocatedIdentityName in allocatedBy)
            {
                var allocatedClient = _clientPool.FirstForIdentityName(allocatedIdentityName);
                var allocatedResponse = password != null ? await allocatedClient.Tokens.GetToken(response.Result!.Id, password) : await allocatedClient.Tokens.GetToken(response.Result!.Id);
                allocatedResponse.Status.ShouldBe(HttpStatusCode.OK);
            }
        }
    }

    private async Task SendInvalidPasswordsToToken(string tokenName, int numberOfInvalidRequests)
    {
        var client = _clientPool.Anonymous;
        var token = _tokensContext.CreateTokenResponses[tokenName];

        for (var i = 0; i < numberOfInvalidRequests; i++)
            await client.Tokens.GetTokenUnauthenticated(token.Id);
    }

    #endregion

    #region When

    [When($"^{RegexFor.SINGLE_THING} sends a POST request to the /Tokens endpoint$")]
    public async Task WhenIdentitySendsAPostRequestToTheTokensEndpoint(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = await client.Tokens.CreateToken(new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW });
    }

    [When("^an anonymous user sends a POST request to the /Tokens endpoint$")]
    public async Task WhenAnAnonymousUserSendsAPOSTRequestIsSentToTheTokensEndpoint()
    {
        _responseContext.WhenResponse = await _clientPool.Anonymous.Tokens.CreateTokenUnauthenticated(new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW });
    }

    [When($@"{RegexFor.SINGLE_THING} sends a POST request to the /Tokens endpoint with the password ""([^""]*)""")]
    public async Task WhenISendsAPOSTRequestToTheTokensEndpointWithThePassword(string identityName, string passwordString)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var password = Encoding.UTF8.GetBytes(passwordString);

        _responseContext.WhenResponse = await client.Tokens.CreateToken(
            new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW, Password = password });
    }

    [When($@"{RegexFor.SINGLE_THING} sends a POST request to the /Tokens endpoint with password ""([^""]*)"" and forIdentity {RegexFor.OPTIONAL_SINGLE_THING}")]
    public async Task WhenISendsAPOSTRequestToTheTokensEndpointWithPasswordAndForIdentityI(string identityName, string passwordString, string forIdentityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var forClient = _clientPool.FirstForIdentityName(forIdentityName);
        var password = Encoding.UTF8.GetBytes(passwordString);

        _responseContext.WhenResponse = await client.Tokens.CreateToken(
            new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW, ForIdentity = forClient.IdentityData!.Address, Password = password });
    }

    [When($@"{RegexFor.OPTIONAL_SINGLE_THING} sends a GET request to the /Tokens/{RegexFor.SINGLE_THING}.Id endpoint with password ""([^""]*)""")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensIdEndpointWithPassword(string identityName, string tokenName, string password)
    {
        var isAuthenticated = identityName != "-";
        var isPasswordProvided = password != "-";

        var client = isAuthenticated ? _clientPool.FirstForIdentityName(identityName) : _clientPool.Anonymous;
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].Id;

        if (isAuthenticated)
            _responseContext.WhenResponse = isPasswordProvided
                ? await client.Tokens.GetToken(tokenId, Convert.FromBase64String(password.Trim()))
                : await client.Tokens.GetToken(tokenId);
        else
            _responseContext.WhenResponse = isPasswordProvided
                ? await client.Tokens.GetTokenUnauthenticated(tokenId, Convert.FromBase64String(password.Trim()))
                : await client.Tokens.GetTokenUnauthenticated(tokenId);
    }

    [When($@"{RegexFor.OPTIONAL_SINGLE_THING} sends a GET request to the /Tokens/{RegexFor.SINGLE_THING}.Id endpoint")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensIdEndpoint(string identityName, string tokenName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].Id;

        _responseContext.WhenResponse = await client.Tokens.GetToken(tokenId);
    }

    [When($@"{RegexFor.SINGLE_THING} sends a GET request to the /Tokens endpoint with the following payloads")]
    public async Task WhenISendsAGETRequestToTheTokensEndpointWithTheFollowingPayloads(string identityName, Table table)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var getRequestPayloadSet = table.CreateSet<GetRequestPayload>();

        var queryItems = getRequestPayloadSet.Select(payload => _tokensContext.CreateTokenResponses[payload.TokenName].Id);

        _responseContext.WhenResponse = _listTokensResponse = await client.Tokens.ListTokens(queryItems);
    }

    [When($"{RegexFor.SINGLE_THING} sends a DELETE request to the /Tokens/{RegexFor.SINGLE_THING}.Id endpoint")]
    public async Task WhenISendsADeleteRequestToTheTokensIdEndpoint(string identityName, string tokenName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].Id;

        _responseContext.WhenResponse = await client.Tokens.DeleteToken(tokenId);
    }

    #endregion

    #region Then

    [Then($@"the response contains Token\(s\) {RegexFor.LIST_OF_THINGS}")]
    public void ThenTheResponseContainsTokens(string tokenNames)
    {
        var tokens = tokenNames
            .Split(',')
            .Select(item => _tokensContext.CreateTokenResponses[item.Trim()])
            .Select(item => (item.Id, item.CreatedAt))
            .ToList();

        _listTokensResponse!.Result!
            .Select(item => (item.Id, item.CreatedAt))
            .ShouldBe(tokens, true);
    }

    #endregion
}

// ReSharper disable once ClassNeverInstantiated.Local
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
file class TokenProperties
{
    public required string TokenName { get; set; }
    public required string TokenOwner { get; set; }
    public required string ForIdentity { get; set; }
    public required string Password { get; set; }
    public required string AllocatedBy { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Local
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
file class GetRequestPayload
{
    public required string TokenName { get; set; }
}

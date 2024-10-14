﻿using System.Diagnostics.CodeAnalysis;
using System.Text;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using TechTalk.SpecFlow.Assist;

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

    [Given($"Token {RegexFor.SINGLE_THING} belonging to {RegexFor.SINGLE_THING} where ForIdentity is the address of {RegexFor.SINGLE_THING}")]
    public async Task GivenTokenTBelongingToIWhereForIdentityIsTheAddressOfI(string tokenName, string identityName, string forIdentityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var forClient = _clientPool.FirstForIdentityName(forIdentityName);

        var response = await client.Tokens.CreateToken(new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW, ForIdentity = forClient.IdentityData!.Address });
        response.Should().BeASuccess();

        _tokensContext.CreateTokenResponses[tokenName] = response.Result!;
    }

    // =-=-=-=

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

    #endregion

    #region When

    //[When($"{RegexFor.SINGLE_THING} sends a GET request to the /Tokens endpoint with the ids of {RegexFor.LIST_OF_THINGS}")]
    //public async Task WhenIdentitySendsAGetRequestToTheTokensEndpointWithAListOfIdsOfOwnTokens(string identityName, string tokenNames)
    //{
    //    var client = _clientPool.FirstForIdentityName(identityName);

    //    var tokenIds = Utils.SplitNames(tokenNames).Select(tokenName => _tokensContext.CreateTokenResponses[tokenName].Id).ToArray();

    //    _responseContext.WhenResponse = _listTokensResponse = await client.Tokens.ListTokens(tokenIds);
    //    _responseContext.WhenResponse.Should().NotBeNull();
    //}

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
        _responseContext.WhenResponse = await _clientPool.Anonymous.Tokens.GetTokenUnauthenticated(tokenId);
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Tokens/{{id}} endpoint with \"([^\"]*)\"")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensIdEndpointWithNonExistingTokenId(string identityName, string nonExistingTokenId)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = await client.Tokens.GetToken(nonExistingTokenId);
    }

    // =-=-=-=

    [When($@"{RegexFor.SINGLE_THING} sends a GET request to the /Tokens/{RegexFor.SINGLE_THING}.Id endpoint with password ""([^""]*)""")]
    public async Task WhenIdentitySendsAGetRequestToTheTokensIdEndpointWithPassword(string identityName, string tokenName, string password)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var tokenId = _tokensContext.CreateTokenResponses[tokenName].Id;

        _responseContext.WhenResponse = password != "-"
            ? await client.Tokens.GetToken(tokenId, Convert.FromBase64String(password.Trim()))
            : await client.Tokens.GetToken(tokenId);
    }

    #endregion

    [Given(@"the following Tokens")]
    public async Task GivenTheFollowingTokens(Table table)
    {
        var tokenPropertiesSet = table.CreateSet<TokenProperties>();

        foreach (var tokenProperties in tokenPropertiesSet)
        {
            var client = _clientPool.FirstForIdentityName(tokenProperties.TokenOwner);
            var forClient = tokenProperties.ForIdentity != "-" ? _clientPool.FirstForIdentityName(tokenProperties.ForIdentity).IdentityData!.Address : null;
            var password = tokenProperties.Password.Trim() != "-" ? Convert.FromBase64String(tokenProperties.Password.Trim()) : null;

            var response = await client.Tokens
                .CreateToken(new CreateTokenRequest { Content = TestData.SOME_BYTES, ExpiresAt = TOMORROW, ForIdentity = forClient, Password = password });

            _tokensContext.CreateTokenResponses[tokenProperties.TokenName] = response.Result!;
        }
    }

    [When($@"{RegexFor.SINGLE_THING} sends a GET request to the /Tokens endpoint with the following payloads")]
    public async Task WhenISendsAGETRequestToTheTokensEndpointWithTheFollowingPayloads(string identityName, Table table)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var getRequestPayloadSet = table.CreateSet<GetRequestPayload>();

        var queryItems = getRequestPayloadSet.Select(payload =>
        {
            var tokenId = _tokensContext.CreateTokenResponses[payload.TokenName].Id;
            var password = payload.PasswordOnGet == "-" ? null : Convert.FromBase64String(payload.PasswordOnGet.Trim());

            return new TokenQueryItem() { Id = tokenId, Password = password };
        }).ToList();

        _responseContext.WhenResponse = _listTokensResponse = await client.Tokens.ListTokens(queryItems);
    }

    [Then($@"the response contains Token\(s\) {RegexFor.LIST_OF_THINGS}")]
    public void ThenTheResponseContainsTokens(string tokenNames)
    {
        var tokens = tokenNames.Split(',').Select(item => _tokensContext.CreateTokenResponses[item.Trim()]).ToList();
        _listTokensResponse!.Result!.Should().BeEquivalentTo(tokens, options => options.WithStrictOrdering());
    }


    #region Then

    [Then($"the response does not contain the Token {RegexFor.SINGLE_THING}")]
    public void ThenTheResponseDoesNotContainTheTokenT(string tokenName)
    {
        var tokenId = _tokensContext.CreateTokenResponses[tokenName].Id;

        _listTokensResponse!.Result.Should().NotContain(token => token.Id == tokenId);
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
}

// ReSharper disable once ClassNeverInstantiated.Local
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
file class GetRequestPayload
{
    public required string TokenName { get; set; }
    public required string PasswordOnGet { get; set; }
}


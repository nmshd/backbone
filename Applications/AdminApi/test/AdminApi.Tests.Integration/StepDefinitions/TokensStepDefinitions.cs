﻿using Backbone.AdminApi.Sdk.Endpoints.Tokens.Response;
using Backbone.AdminApi.Sdk.Services;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;
using PaginationFilter = Backbone.BuildingBlocks.SDK.Endpoints.Common.Types.PaginationFilter;


namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET /Tokens?createdBy={identity-address}")]
[Scope(Feature = "PATCH /Tokens/{id}/ResetAccessFailedCount")]
internal class TokensStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : BaseStepDefinitions(factory, options)
{
    private IResponse? _whenResponse = null;
    private ApiResponse<ListTokensResponse> _listTokensResponse = null!;
    private ApiResponse<EmptyResponse> _resetAccesesFailedCountResponse = null!;
    private string _newIdentityAddress = string.Empty;


    [Given(@"an identity with no tokens")]
    public async Task GivenAnIdentityWithNoTokens()
    {
        var createIdentityResponse = await IdentityCreationHelper.CreateIdentity(_client);

        createIdentityResponse.ShouldBeASuccess();

        _newIdentityAddress = createIdentityResponse.Result!.Address;
    }

    [When("^a GET request is sent to the /Tokens endpoint with the identity's address$")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithTheIdentitysAddress()
    {
        _whenResponse = _listTokensResponse = await _client.Tokens.ListTokensByIdentity(new PaginationFilter { PageNumber = 1, PageSize = 5 }, _newIdentityAddress, CancellationToken.None);
    }

    [When("^a PATCH request is sent to the /Tokens/TOKANonExistingIdxxx/ResetAccessFailedCount endpoint$")]
    public async Task WhenAPATCHRequestIsSentToTheTokensTokaNonExistingIdxxxResetAccessFailedCountEndpoint()
    {
        _whenResponse = _resetAccesesFailedCountResponse = await _client.Tokens.ResetAccessFailedCount("TOKANonExistingIdxxx", CancellationToken.None);
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        _whenResponse.ShouldNotBeNull();
        ((int)_whenResponse!.Status).ShouldBe(expectedStatusCode);
    }

    [Then(@"the response content is an empty array")]
    public void ThenTheResponseContentIsAnEmptyArray()
    {
        _listTokensResponse.ShouldNotBeNull();
        _listTokensResponse.Result!.Count().ShouldBe(0);
    }
}

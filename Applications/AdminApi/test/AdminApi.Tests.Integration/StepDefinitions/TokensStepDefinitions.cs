using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.AdminApi.Sdk.Endpoints.Tokens.Response;
using Backbone.AdminApi.Sdk.Services;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;
using PaginationFilter = Backbone.BuildingBlocks.SDK.Endpoints.Common.Types.PaginationFilter;


namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET /Tokens?createdBy={identity-address}")]
internal class TokensStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : BaseStepDefinitions(factory, options)
{
    private ApiResponse<CreateIdentityResponse>? _createIdentityResponse;
    private string _newIdentity = string.Empty;
    private ApiResponse<ListTokensTestResponse> _tokensResult = null!;

    [Given(@"an identity with no tokens")]
    public async Task GivenAnIdentityWithNoTokens()
    {
        _createIdentityResponse = await IdentityCreationHelper.CreateIdentity(_client);

        _createIdentityResponse.Should().BeASuccess();

        _newIdentity = _createIdentityResponse.Result!.Address;
    }


    [When(@"a GET request is sent to the /Tokens endpoint with the identity's address")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithTheIdentitysAddress()
    {
        _tokensResult = await _client.Tokens.ListTokensByIdentity(new PaginationFilter { PageNumber = 1, PageSize = 5 }, _newIdentity, CancellationToken.None);
    }


    //_tokensResult.Result!.Pagination!.TotalRecords.Should().Be(0);

    [Then(@"the response content is an empty array")]
    public void ThenTheResponseContentIsAnEmptyArray()
    {
        var tokens = _tokensResult.Result!.Count;
        tokens.Should().Be(0);
    }
}

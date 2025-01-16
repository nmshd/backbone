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
    private ApiResponse<ListTokensTestResponse> _listTokensTestResponse = null!;
    private string _newIdentityAddress = string.Empty;

    [Given(@"an identity with no tokens")]
    public async Task GivenAnIdentityWithNoTokens()
    {
        var createIdentityResponse = await IdentityCreationHelper.CreateIdentity(_client);

        createIdentityResponse.Should().BeASuccess();

        _newIdentityAddress = createIdentityResponse.Result!.Address;
    }


    [When(@"a GET request is sent to the /Tokens endpoint with the identity's address")]
    public async Task WhenAGETRequestIsSentToTheTokensEndpointWithTheIdentitysAddress()
    {
        _listTokensTestResponse = await _client.Tokens.ListTokensByIdentity(new PaginationFilter { PageNumber = 1, PageSize = 5 }, _newIdentityAddress, CancellationToken.None);
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_listTokensTestResponse!.Status).Should().Be(expectedStatusCode);
    }


    [Then(@"the response content is an empty array")]
    public void ThenTheResponseContentIsAnEmptyArray()
    {
        var tokens = _listTokensTestResponse.Result!.Count;
        tokens.Should().Be(0);
    }
}

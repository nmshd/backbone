using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Responses;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.UnitTestTools.Data;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.SdkStepDefinitions;

[Binding]
[Scope(Feature = "GET Tiers")]
[Scope(Feature = "POST Tier")]
[Scope(Feature = "DELETE Tier")]
internal class TiersStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<Tier>? _tierResponse;
    private ApiResponse<EmptyResponse>? _deleteResponse;
    private ApiResponse<ListTiersResponse>? _tiersResponse;
    private string _existingTierName;
    private string _existingTierId;

    public TiersStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
        _existingTierName = string.Empty;
        _existingTierId = string.Empty;
    }

    [Given("a Tier t")]
    public async Task GivenATier()
    {
        var response = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + TestDataGenerator.GenerateString(12) });
        response.IsSuccess.Should().BeTrue();

        _existingTierName = response.Result!.Name;
        _existingTierId = response.Result.Id;
    }

    [Given("the Tier T has one associated identity")]
    public void GivenTheTierTHasOneAssociatedIdentity()
    {
        throw new PendingStepException();
    }

    [Given("the Basic Tier as t")]
    public async Task GivenTheBasicTierAsT()
    {
        var response = await _client.Tiers.ListTiers();
        response.IsSuccess.Should().BeTrue();

        var basicTier = response.Result!.Single(t => t.Name == "Basic");
        _existingTierName = basicTier.Name;
        _existingTierId = basicTier.Id;
    }

    [When("a GET request is sent to the /Tiers endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersEndpoint()
    {
        _tiersResponse = await _client.Tiers.ListTiers();
    }

    [When("a POST request is sent to the /Tiers endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheTiersEndpoint()
    {
        _tierResponse = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + TestDataGenerator.GenerateString(12) });
    }

    [When("a POST request is sent to the /Tiers endpoint with the name t.Name")]
    public async Task WhenAPOSTRequestIsSentToTheTiersEndpointWithAnAlreadyExistingName()
    {
        _tierResponse = await _client.Tiers.CreateTier(new CreateTierRequest { Name = _existingTierName });
    }

    [When(@"a DELETE request is sent to the /Tiers/\{t\.Id} endpoint")]
    public async Task WhenADeleteRequestIsSentToTheTiersTierIdEndpoint()
    {
        _deleteResponse = await _client.Tiers.DeleteTier(_existingTierId);
    }

    [When(@"a DELETE request is sent to the /Tiers/\{t\.Id} endpoint with an inexistent id")]
    public async Task WhenADeleteRequestIsSentToTheTiersT_IdEndpointWithAnInexistentId()
    {
        _deleteResponse = await _client.Tiers.DeleteTier("TIR00000000000000000");
    }

    [Then("the response contains a paginated list of Tiers")]
    public void ThenTheResponseContainsAListOfTiers()
    {
        _tiersResponse!.IsSuccess.Should().BeTrue();
        _tiersResponse!.AssertContentCompliesWithSchema();
    }

    [Then("the response contains a Tier")]
    public void ThenTheResponseContainsATier()
    {
        _tierResponse!.IsSuccess.Should().BeTrue();
        _tierResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_tierResponse != null)
            ((int)_tierResponse!.Status).Should().Be(expectedStatusCode);

        if (_tiersResponse != null)
            ((int)_tiersResponse!.Status).Should().Be(expectedStatusCode);

        if (_deleteResponse != null)
            ((int)_deleteResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _tierResponse!.Error.Should().NotBeNull();
        _tierResponse.Error!.Code.Should().Be(errorCode);
    }
}

using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Responses;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Tiers")]
[Scope(Feature = "GET Tiers/{id}")]
[Scope(Feature = "POST Tier")]
[Scope(Feature = "DELETE Tier")]
internal class TiersStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<Tier>? _tierResponse;
    private ApiResponse<TierDetails>? _getTierResponse;
    private ApiResponse<EmptyResponse>? _deleteResponse;
    private ApiResponse<ListTiersResponse>? _tiersResponse;
    private IResponse? _whenResponse;
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
        var response = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + CreateRandomString(12) });
        response.ShouldBeASuccess();

        _existingTierName = response.Result!.Name;
        _existingTierId = response.Result.Id;
    }

    [Given("the Basic Tier as t")]
    public async Task GivenTheBasicTierAsT()
    {
        var response = await _client.Tiers.ListTiers();
        response.ShouldBeASuccess();

        var basicTier = response.Result!.Single(t => t.Name == "Basic");
        _existingTierName = basicTier.Name;
        _existingTierId = basicTier.Id;
    }

    [When("^a GET request is sent to the /Tiers endpoint$")]
    public async Task WhenAGETRequestIsSentToTheTiersEndpoint()
    {
        _whenResponse = _tiersResponse = await _client.Tiers.ListTiers();
    }

    [When("^a GET request is sent to the /Tiers/{t.id} endpoint$")]
    public async Task WhenAGETRequestIsSentToTheTiersByIdEndpoint()
    {
        _whenResponse = _getTierResponse = await _client.Tiers.GetTier(_existingTierId);
    }

    [When("^a GET request is sent to the /Tiers/{nonExistentTierId} endpoint$")]
    public async Task WhenAGETRequestIsSentToTheTiersByIdEndpointWithANonExistentId()
    {
        _whenResponse = _getTierResponse = await _client.Tiers.GetTier("TIRNonExistentId1231");
    }

    [When("^a POST request is sent to the /Tiers endpoint$")]
    public async Task WhenAPOSTRequestIsSentToTheTiersEndpoint()
    {
        _whenResponse = _tierResponse = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + CreateRandomString(12) });
    }

    [When("^a POST request is sent to the /Tiers endpoint with the name t.Name$")]
    public async Task WhenAPOSTRequestIsSentToTheTiersEndpointWithAnAlreadyExistingName()
    {
        _whenResponse = _tierResponse = await _client.Tiers.CreateTier(new CreateTierRequest { Name = _existingTierName });
    }

    [When("^a DELETE request is sent to the /Tiers/{t.Id} endpoint$")]
    public async Task WhenADeleteRequestIsSentToTheTiersTierIdEndpoint()
    {
        _whenResponse = _deleteResponse = await _client.Tiers.DeleteTier(_existingTierId);
    }

    [When("^a DELETE request is sent to the /Tiers/{t.Id} endpoint with an inexistent id$")]
    public async Task WhenADeleteRequestIsSentToTheTiersT_IdEndpointWithAnInexistentId()
    {
        _whenResponse = _deleteResponse = await _client.Tiers.DeleteTier("TIR00000000000000000");
    }

    [Then("the response contains a paginated list of Tiers")]
    public async Task ThenTheResponseContainsAListOfTiers()
    {
        _tiersResponse!.ShouldBeASuccess();
        _tiersResponse!.ContentType.ShouldStartWith("application/json");
        await _tiersResponse.ShouldComplyWithSchema();
    }

    [Then("the response contains a Tier")]
    public async Task ThenTheResponseContainsATier()
    {
        _tierResponse!.ShouldBeASuccess();
        _tierResponse!.ContentType.ShouldStartWith("application/json");
        await _tierResponse.ShouldComplyWithSchema();
    }

    [Then("the response contains Tier t")]
    public async Task ThenTheResponseContainsTierT()
    {
        _getTierResponse!.ShouldBeASuccess();
        _getTierResponse!.ContentType.ShouldStartWith("application/json");
        await _getTierResponse.ShouldComplyWithSchema();
        _getTierResponse.Result!.Id.ShouldBe(_existingTierId);
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        _whenResponse.ShouldNotBeNull();
        ((int)_whenResponse!.Status).ShouldBe(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _whenResponse.ShouldNotBeNull();
        _whenResponse!.Error.ShouldNotBeNull();
        _whenResponse.Error!.Code.ShouldBe(errorCode);
    }
}

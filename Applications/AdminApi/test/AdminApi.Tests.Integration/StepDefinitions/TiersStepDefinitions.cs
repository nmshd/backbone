﻿using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;
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
        response.Should().BeASuccess();

        _existingTierName = response.Result!.Name;
        _existingTierId = response.Result.Id;
    }

    [Given("the Basic Tier as t")]
    public async Task GivenTheBasicTierAsT()
    {
        var response = await _client.Tiers.ListTiers();
        response.Should().BeASuccess();

        var basicTier = response.Result!.Single(t => t.Name == "Basic");
        _existingTierName = basicTier.Name;
        _existingTierId = basicTier.Id;
    }

    [When("a GET request is sent to the /Tiers endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersEndpoint()
    {
        _tiersResponse = await _client.Tiers.ListTiers();
    }

    [When("a GET request is sent to the /Tiers/{t.id} endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersByIdEndpoint()
    {
        _getTierResponse = await _client.Tiers.GetTier(_existingTierId);
    }

    [When("a GET request is sent to the /Tiers/{nonExistentTierId} endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersByIdEndpointWithANonExistentId()
    {
        _getTierResponse = await _client.Tiers.GetTier("TIRNonExistentId1231");
    }

    [When("a POST request is sent to the /Tiers endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheTiersEndpoint()
    {
        _tierResponse = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + CreateRandomString(12) });
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
    public async Task ThenTheResponseContainsAListOfTiers()
    {
        _tiersResponse!.Should().BeASuccess();
        _tiersResponse!.ContentType.Should().StartWith("application/json");
        await _tiersResponse.Should().ComplyWithSchema();
    }

    [Then("the response contains a Tier")]
    public async Task ThenTheResponseContainsATier()
    {
        _tierResponse!.Should().BeASuccess();
        _tierResponse!.ContentType.Should().StartWith("application/json");
        await _tierResponse.Should().ComplyWithSchema();
    }

    [Then("the response contains Tier t")]
    public async Task ThenTheResponseContainsTierT()
    {
        _getTierResponse!.Should().BeASuccess();
        _getTierResponse!.ContentType.Should().StartWith("application/json");
        await _getTierResponse.Should().ComplyWithSchema();
        _getTierResponse.Result!.Id.Should().Be(_existingTierId);
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

        if (_getTierResponse != null)
            ((int)_getTierResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_tierResponse != null)
        {
            _tierResponse!.Error.Should().NotBeNull();
            _tierResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_getTierResponse != null)
        {
            _getTierResponse!.Error.Should().NotBeNull();
            _getTierResponse.Error!.Code.Should().Be(errorCode);
        }
    }
}

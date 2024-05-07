using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Requests;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.UnitTestTools.Data;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST TierQuota")]
[Scope(Feature = "DELETE TierQuota")]
internal class TierQuotaStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<TierQuotaDefinition>? _createTierQuotaResponse;
    private ApiResponse<EmptyResponse>? _deleteResponse;
    private string _tierId;
    private string _tierQuotaDefinitionId;

    public TierQuotaStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
        _tierId = string.Empty;
        _tierQuotaDefinitionId = string.Empty;
    }

    [Given("a Tier t")]
    public async Task GivenAValidTier()
    {
        var response = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + TestDataGenerator.GenerateString(12) });
        response.IsSuccess.Should().BeTrue();

        _tierId = response.Result!.Id;

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);
    }

    [Given("a Tier t with a Quota q")]
    public async Task GivenAValidTierWithAQuota()
    {
        await GivenAValidTier();

        var response = await _client.Tiers.AddTierQuota(_tierId, new CreateQuotaForTierRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        });
        response.IsSuccess.Should().BeTrue();

        _tierQuotaDefinitionId = response.Result!.Id;

        // allow the event queue to trigger the creation of this tier quota definition on the Quotas module
        Thread.Sleep(2000);
    }

    [When("a POST request is sent to the /Tiers/{t.id}/Quotas endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheCreateTierQuotaEndpoint()
    {
        _createTierQuotaResponse = await _client.Tiers.AddTierQuota(_tierId, new CreateQuotaForTierRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        });
    }

    [When("a POST request is sent to the /Tiers/{tierId}/Quotas endpoint with an inexistent tier id")]
    public async Task WhenAPOSTRequestIsSentToTheCreateTierQuotaEndpointForAnInexistentTier()
    {
        _createTierQuotaResponse = await _client.Tiers.AddTierQuota("inexistentTierId", new CreateQuotaForTierRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        });
    }

    [When("a DELETE request is sent to the /Tiers/{t.id}/Quotas/{q.id} endpoint")]
    public async Task WhenADeleteRequestIsSentToTheDeleteTierQuotaEndpoint()
    {
        _deleteResponse = await _client.Tiers.DeleteTierQuota(_tierId, _tierQuotaDefinitionId);
    }

    [When("a DELETE request is sent to the /Tiers/{t.id}/Quotas/{quotaId} endpoint with an inexistent quota id")]
    public async Task WhenADeleteRequestIsSentToTheDeleteTierQuotaEndpointForAnInexistentQuota()
    {
        _deleteResponse = await _client.Tiers.DeleteTierQuota(_tierId, "inexistentQuotaId");
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_createTierQuotaResponse != null)
            ((int)_createTierQuotaResponse!.Status).Should().Be(expectedStatusCode);

        if (_deleteResponse != null)
            ((int)_deleteResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then("the response contains a TierQuota")]
    public void ThenTheResponseContainsATierQuotaDefinition()
    {
        _createTierQuotaResponse!.IsSuccess.Should().BeTrue();
        _createTierQuotaResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_createTierQuotaResponse != null)
        {
            _createTierQuotaResponse!.Error.Should().NotBeNull();
            _createTierQuotaResponse.Error!.Code.Should().Be(errorCode);
        }

        if (_deleteResponse != null)
        {
            _deleteResponse!.Error.Should().NotBeNull();
            _deleteResponse.Error!.Code.Should().Be(errorCode);
            ;
        }
    }
}

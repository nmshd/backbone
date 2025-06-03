using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Requests;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST TierQuota")]
[Scope(Feature = "DELETE TierQuota")]
internal class TierQuotaStepDefinitions : BaseStepDefinitions
{
    private IResponse? _whenResponse;
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
        var response = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + CreateRandomString(12) });
        response.ShouldBeASuccess();

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
        response.ShouldBeASuccess();

        _tierQuotaDefinitionId = response.Result!.Id;

        // allow the event queue to trigger the creation of this tier quota definition on the Quotas module
        Thread.Sleep(2000);
    }

    [When("^a POST request is sent to the /Tiers/{t.id}/Quotas endpoint$")]
    public async Task WhenAPOSTRequestIsSentToTheCreateTierQuotaEndpoint()
    {
        _whenResponse = _createTierQuotaResponse = await _client.Tiers.AddTierQuota(_tierId, new CreateQuotaForTierRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        });
    }

    [When("^a POST request is sent to the /Tiers/{t.id}/Quotas endpoint with an invalid metric key$")]
    public async Task WhenAPOSTRequestIsSentToTheCreateTierQuotaEndpointWithAnInvalidMetricKey()
    {
        _whenResponse = _createTierQuotaResponse = await _client.Tiers.AddTierQuota(_tierId, new CreateQuotaForTierRequest
        {
            MetricKey = "SomeInvalidMetricKey",
            Max = 2,
            Period = "Week"
        });
    }

    [When("^a POST request is sent to the /Tiers/{tierId}/Quotas endpoint with an inexistent tier id$")]
    public async Task WhenAPOSTRequestIsSentToTheCreateTierQuotaEndpointForAnInexistentTier()
    {
        _whenResponse = _createTierQuotaResponse = await _client.Tiers.AddTierQuota("inexistentTierId", new CreateQuotaForTierRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        });
    }

    [When("^a DELETE request is sent to the /Tiers/{t.id}/Quotas/{q.id} endpoint$")]
    public async Task WhenADeleteRequestIsSentToTheDeleteTierQuotaEndpoint()
    {
        _whenResponse = _deleteResponse = await _client.Tiers.DeleteTierQuota(_tierId, _tierQuotaDefinitionId);
    }

    [When("^a DELETE request is sent to the /Tiers/{t.id}/Quotas/{quotaId} endpoint with an inexistent quota id$")]
    public async Task WhenADeleteRequestIsSentToTheDeleteTierQuotaEndpointForAnInexistentQuota()
    {
        _whenResponse = _deleteResponse = await _client.Tiers.DeleteTierQuota(_tierId, "inexistentQuotaId");
    }

    [When("^a DELETE request is sent to the /Tiers/{nonExistentTier}/Quotas/{q.id}$")]
    public async Task WhenADeleteRequestIsSentToTheDeleteTierQuotaEndpointWithANonExistentTierId()
    {
        _whenResponse = _deleteResponse = await _client.Tiers.DeleteTierQuota("nonExistentTierId", _tierQuotaDefinitionId);
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        _whenResponse.ShouldNotBeNull();
        ((int)_whenResponse!.Status).ShouldBe(expectedStatusCode);
    }

    [Then("the response contains a TierQuota")]
    public async Task ThenTheResponseContainsATierQuotaDefinition()
    {
        _createTierQuotaResponse!.ShouldBeASuccess();
        _createTierQuotaResponse!.ContentType.ShouldStartWith("application/json");
        await _createTierQuotaResponse.ShouldComplyWithSchema();
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _whenResponse.ShouldNotBeNull();
        _whenResponse!.Error.ShouldNotBeNull();
        _whenResponse.Error!.Code.ShouldBe(errorCode);
    }
}

using Backbone.AdminApi.Tests.Integration.API;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.AdminApi.Tests.Integration.Models;
using Backbone.UnitTestTools.Data;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Scope(Feature = "POST TierQuota")]
[Scope(Feature = "DELETE TierQuota")]
internal class TierQuotaStepDefinitions : BaseStepDefinitions
{
    private readonly TiersApi _tiersApi;
    private string _tierId;
    private string _tierQuotaDefinitionId;
    private HttpResponse<TierQuotaDTO>? _response;
    private HttpResponse? _deleteResponse;

    public TierQuotaStepDefinitions(TiersApi tiersApi)
    {
        _tiersApi = tiersApi;
        _tierId = string.Empty;
        _tierQuotaDefinitionId = string.Empty;
    }

    [Given("a Tier t")]
    public async Task GivenAValidTier()
    {
        var createTierRequest = new CreateTierRequest
        {
            Name = "TestTier_" + TestDataGenerator.GenerateString(12)
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierRequest);

        var response = await _tiersApi.CreateTier(requestConfiguration);

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);
        _tierId = response.Content.Result!.Id;

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);
    }

    [Given("a Tier t with a Quota q")]
    public async Task GivenAValidTierWithAQuota()
    {
        await GivenAValidTier();

        var createTierQuotaRequest = new CreateTierQuotaRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierQuotaRequest);

        var response = await _tiersApi.CreateTierQuota(requestConfiguration, _tierId);

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);
        _tierQuotaDefinitionId = response.Content.Result!.Id;

        // allow the event queue to trigger the creation of this tier quota definition on the Quotas module
        Thread.Sleep(2000);
    }

    [When("a POST request is sent to the /Tiers/{t.id}/Quotas endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheCreateTierQuotaEndpoint()
    {
        var createTierQuotaRequest = new CreateTierQuotaRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierQuotaRequest);

        _response = await _tiersApi.CreateTierQuota(requestConfiguration, _tierId);
    }

    [When("a POST request is sent to the /Tiers/{tierId}/Quotas endpoint with an inexistent tier id")]
    public async Task WhenAPOSTRequestIsSentToTheCreateTierQuotaEndpointForAnInexistentTier()
    {
        var createTierQuotaRequest = new CreateTierQuotaRequest
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierQuotaRequest);

        _response = await _tiersApi.CreateTierQuota(requestConfiguration, "inexistentTierId");
    }

    [When("a DELETE request is sent to the /Tiers/{t.id}/Quotas/{q.id} endpoint")]
    public async Task WhenADELETERequestIsSentToTheDeleteTierQuotaEndpoint()
    {
        _deleteResponse = await _tiersApi.DeleteTierQuota(_tierId, _tierQuotaDefinitionId, _requestConfiguration);
        _deleteResponse.Should().NotBeNull();
    }

    [When("a DELETE request is sent to the /Tiers/{t.id}/Quotas/{quotaId} endpoint with an inexistent quota id")]
    public async Task WhenADELETERequestIsSentToTheDeleteTierQuotaEndpointForAnInexistentQuota()
    {
        _deleteResponse = await _tiersApi.DeleteTierQuota(_tierId, "inexistentQuotaId", _requestConfiguration);
        _deleteResponse.Should().NotBeNull();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_response != null)
        {
            var actualStatusCode = (int)_response.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_deleteResponse != null)
        {
            var actualStatusCode = (int)_deleteResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }
    }

    [Then("the response contains a TierQuota")]
    public void ThenTheResponseContainsATierQuotaDefinition()
    {
        _response!.AssertHasValue();
        _response!.AssertStatusCodeIsSuccess();
        _response!.AssertContentTypeIs("application/json");
        _response!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_response != null)
        {
            _response!.Content.Error.Should().NotBeNull();
            _response.Content.Error!.Code.Should().Be(errorCode);
        }

        if (_deleteResponse != null)
        {
            _deleteResponse.Content.Should().NotBeNull();
            _deleteResponse.Content!.Error.Should().NotBeNull();
            _deleteResponse.Content!.Error.Code.Should().Be(errorCode);
        }
    }
}

using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Extensions;
using AdminUi.Tests.Integration.Models;
using Backbone.Modules.Quotas.Domain.Tests;

namespace AdminUi.Tests.Integration.StepDefinitions;

[Scope(Feature = "POST TierQuota")]
[Scope(Feature = "DELETE TierQuota")]
public class TierQuotaStepDefinitions : BaseStepDefinitions
{
    private readonly TiersApi _tiersApi;
    private string _tierId;
    private string _tierQuotaDefinitionId;
    private HttpResponse<TierQuotaDTO>? _response;

    public TierQuotaStepDefinitions(TiersApi tiersApi)
    {
        _tiersApi = tiersApi;
        _tierId = string.Empty;
    }

    [Given(@"a Tier t")]
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

    [Given(@"a TierQuotaDefinition tq")]
    public async Task GivenAValidTierQuotaDefinition()
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

        var response = await _tiersApi.CreateTierQuota(requestConfiguration, _tierId);

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);
        _tierQuotaDefinitionId = response.Content.Result!.Id;

        // allow the event queue to trigger the creation of this tier quota definition on the Quotas module
        Thread.Sleep(2000);
    }

    [Given(@"an inexistent Tier t")]
    public void GivenAnInexistentTier()
    {
        _tierId = "some-inexistent-tier-id";
    }

    [Given(@"an inexistent TierQuotaDefinition tq")]
    public void GivenANonExistentTierQuotaDefinitionTq()
    {
        _tierQuotaDefinitionId = "some-inexistent-tier-quota-definition-id";
    }

    [When(@"a POST request is sent to the /Tier/{t.id}/Quotas endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheCreateTiersQuotaEndpoint()
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

    [When(@"a DELETE request is sent to the /Tier/{t.id}/Quotas/{tq.id} endpoint")]
    public async Task WhenADELETERequestIsSentToTheDeleteTierQuotaEndpoint()
    {
        _response = await _tiersApi.DeleteTierQuota(_tierId, _tierQuotaDefinitionId, _requestConfiguration);
        _response.Should().NotBeNull();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"the response contains a TierQuota")]
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
        _response!.Content.Error.Should().NotBeNull();
        _response.Content.Error!.Code.Should().Be(errorCode);
    }
}

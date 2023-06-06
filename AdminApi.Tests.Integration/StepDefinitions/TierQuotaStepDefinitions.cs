using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Extensions;
using AdminApi.Tests.Integration.Models;
using Backbone.Modules.Quotas.Domain.Tests;
using Newtonsoft.Json;

namespace AdminApi.Tests.Integration.StepDefinitions;

[Scope(Feature = "POST TierQuota")]
public class TierQuotaStepDefinitions : BaseStepDefinitions
{
    private readonly TiersApi _tiersApi;
    private string _tierId;
    private HttpResponse<TierQuotaDefinitionDTO>? _response;

    public TierQuotaStepDefinitions(TiersApi tiersApi) : base()
    {
        _tiersApi = tiersApi;
        _tierId = string.Empty;
    }

    [Given(@"a Tier")]
    public async Task GivenAValidTierAsync()
    {
        var createTierQuotaRequest = new CreateTierRequest
        {
            Name = "TestTier_" + TestDataGenerator.GenerateString(12)
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.Content = JsonConvert.SerializeObject(createTierQuotaRequest);

        var response = await _tiersApi.CreateTier(requestConfiguration);

        var actualStatusCode = (int)response!.StatusCode;
        actualStatusCode.Should().Be(201);
        _tierId = response.Content.Result.Id;

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);

        return;
    }

    [Given(@"an inexistent Tier")]
    public void GivenAnInexistentTier()
    {
        _tierId = "some-inexistent-tier-id";
    }

    [When(@"a POST request is sent to the Create Tier Quota endpoint")]
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
        requestConfiguration.Content = JsonConvert.SerializeObject(createTierQuotaRequest);

        _response = await _tiersApi.CreateTierQuota(requestConfiguration, _tierId);
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"the response contains a TierQuotaDefinition")]
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
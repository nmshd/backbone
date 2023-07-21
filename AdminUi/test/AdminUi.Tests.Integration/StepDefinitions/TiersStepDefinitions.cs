using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Extensions;
using AdminUi.Tests.Integration.Models;
using Backbone.Modules.Quotas.Domain.Tests;

namespace AdminUi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Tiers")]
[Scope(Feature = "POST Tier")]
public class TiersStepDefinitions : BaseStepDefinitions
{
    private readonly TiersApi _tiersApi;
    private HttpResponse<TierDTO>? _tierResponse;
    private HttpResponse<List<TierDTO>>? _tiersResponse;
    private string _tierName;

    public TiersStepDefinitions(TiersApi tiersApi)
    {
        _tiersApi = tiersApi;
        _tierName = string.Empty;
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
        _tierName = response.Content.Result!.Name;
    }

    [When(@"a GET request is sent to the /Tiers endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersEndpoint()
    {
        _tiersResponse = await _tiersApi.GetTiers(_requestConfiguration);
        _tiersResponse.Should().NotBeNull();
        _tiersResponse.Content.Should().NotBeNull();
    }

    [When(@"a POST request is sent to the /Tiers endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheTiersEndpoint()
    {
        var createTierRequest = new CreateTierRequest
        {
            Name = "TestTier_" + TestDataGenerator.GenerateString(12)
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierRequest);

        _tierResponse = await _tiersApi.CreateTier(requestConfiguration);
    }

    [When(@"a POST request is sent to the /Tiers endpoint with the name t.Name")]
    public async Task WhenAPOSTRequestIsSentToTheTiersEndpointWithAnAlreadyExistingName()
    {
        var createTierRequest = new CreateTierRequest
        {
            Name = _tierName
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierRequest);

        _tierResponse = await _tiersApi.CreateTier(requestConfiguration);
    }

    [Then(@"the response contains a paginated list of Tiers")]
    public void ThenTheResponseContainsAList()
    {
        _tiersResponse!.Content.Result.Should().NotBeNull();
        _tiersResponse!.Content.Result.Should().NotBeEmpty();
    }

    [Then(@"the response contains a Tier")]
    public void ThenTheResponseContainsATier()
    {
        _tierResponse!.AssertHasValue();
        _tierResponse!.AssertStatusCodeIsSuccess();
        _tierResponse!.AssertContentTypeIs("application/json");
        _tierResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_tierResponse != null)
        {
            var actualStatusCode = (int)_tierResponse!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }

        if (_tiersResponse != null)
        {
            var actualStatusCode = (int)_tiersResponse!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _tierResponse!.Content.Error.Should().NotBeNull();
        _tierResponse.Content.Error!.Code.Should().Be(errorCode);
    }
}

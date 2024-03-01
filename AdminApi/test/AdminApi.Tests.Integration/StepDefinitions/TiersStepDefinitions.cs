using System.Net;
using Backbone.AdminApi.Tests.Integration.API;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.AdminApi.Tests.Integration.Models;
using Backbone.UnitTestTools.Data;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Tiers")]
[Scope(Feature = "POST Tier")]
[Scope(Feature = "DELETE Tier")]
internal class TiersStepDefinitions : BaseStepDefinitions
{
    private readonly TiersApi _tiersApi;
    private HttpResponse<TierDTO>? _tierResponse;
    private HttpResponse? _deleteResponse;
    private HttpResponse<List<TierOverviewDTO>>? _tiersResponse;
    private string _existingTierName;
    private string _existingTierId;

    public TiersStepDefinitions(TiersApi tiersApi)
    {
        _tiersApi = tiersApi;
        _existingTierName = string.Empty;
        _existingTierId = string.Empty;
    }

    [Given("a Tier t")]
    public async Task GivenATier()
    {
        var createTierRequest = new CreateTierRequest
        {
            Name = "TestTier_" + TestDataGenerator.GenerateString(12)
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierRequest);

        var response = await _tiersApi.CreateTier(requestConfiguration);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        _existingTierName = response.Content.Result!.Name;
        _existingTierId = response.Content.Result!.Id;
    }

    [Given("the Tier T has one associated identity")]
    public void GivenTheTierTHasOneAssociatedIdentity()
    {
        throw new PendingStepException();
    }

    [Given("the Basic Tier as t")]
    public async Task GivenTheBasicTierAsT()
    {
        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";

        var response = await _tiersApi.GetTiers(requestConfiguration);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var basicTier = response.Content.Result!.Single(t => t.Name == "Basic");
        _existingTierName = basicTier.Name;
        _existingTierId = basicTier.Id;
    }

    [When("a GET request is sent to the /Tiers endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersEndpoint()
    {
        _tiersResponse = await _tiersApi.GetTiers(_requestConfiguration);
        _tiersResponse.Should().NotBeNull();
        _tiersResponse.Content.Should().NotBeNull();
    }

    [When("a POST request is sent to the /Tiers endpoint")]
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

    [When("a POST request is sent to the /Tiers endpoint with the name t.Name")]
    public async Task WhenAPOSTRequestIsSentToTheTiersEndpointWithAnAlreadyExistingName()
    {
        var createTierRequest = new CreateTierRequest
        {
            Name = _existingTierName
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierRequest);

        _tierResponse = await _tiersApi.CreateTier(requestConfiguration);
    }

    [When(@"a DELETE request is sent to the /Tiers/\{t\.Id} endpoint")]
    public async Task WhenADELETERequestIsSentToTheTiersTierIdEndpoint()
    {
        _deleteResponse = await _tiersApi.DeleteTier(_requestConfiguration, _existingTierId);
    }

    [When(@"a DELETE request is sent to the /Tiers/\{t\.Id} endpoint with an inexistent id")]
    public async Task WhenADELETERequestIsSentToTheTiersT_IdEndpointWithAnInexistentId()
    {
        _deleteResponse = await _tiersApi.DeleteTier(_requestConfiguration, "TIR00000000000000000");
    }

    [Then("the response contains a paginated list of Tiers")]
    public void ThenTheResponseContainsAList()
    {
        _tiersResponse!.Content.Result.Should().NotBeNull();
        _tiersResponse!.Content.Result.Should().NotBeEmpty();
    }

    [Then("the response contains a Tier")]
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

        if (_deleteResponse != null)
        {
            var actualStatusCode = (int)_deleteResponse!.StatusCode;
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

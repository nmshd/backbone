using Backbone.AdminApi.Tests.Integration.API;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.AdminApi.Tests.Integration.Models;
using Backbone.UnitTestTools.Data;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Tier Details")]
internal class TierDetailsStepDefinitions : BaseStepDefinitions
{
    private readonly TiersApi _tiersApi;
    private string _tierId;
    private HttpResponse<TierDetailsDTO>? _response;

    public TierDetailsStepDefinitions(TiersApi tiersApi)
    {
        _tiersApi = tiersApi;
        _tierId = string.Empty;
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

        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);
        _tierId = response.Content.Result!.Id;

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);
    }

    [When("a GET request is sent to the /Tiers/{t.id} endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersIdEndpoint()
    {
        _response = await _tiersApi.GetTierById(_requestConfiguration, _tierId);
        _response.Should().NotBeNull();
        _response.Content.Should().NotBeNull();
    }

    [Then("the response contains Tier t")]
    public void ThenTheResponseContainsATier()
    {
        _response!.Content.Result.Should().NotBeNull();
        _response!.Content.Result!.Id.Should().NotBeNull();
        _response!.Content.Result!.Id.Should().Be(_tierId);
        _response!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }
}

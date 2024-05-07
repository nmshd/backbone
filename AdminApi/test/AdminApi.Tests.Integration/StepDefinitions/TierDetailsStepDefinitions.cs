using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Requests;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.UnitTestTools.Data;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Tier Details")]
internal class TierDetailsStepDefinitions : BaseStepDefinitions
{
    private string _tierId;
    private ApiResponse<TierDetails>? _tierDetailsResponse;

    public TierDetailsStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
        _tierId = string.Empty;
    }

    [Given("a Tier t")]
    public async Task GivenATier()
    {
        var response = await _client.Tiers.CreateTier(new CreateTierRequest { Name = "TestTier_" + TestDataGenerator.GenerateString(12) });
        response.IsSuccess.Should().BeTrue();
        _tierId = response.Result!.Id;

        // allow the event queue to trigger the creation of this tier on the Quotas module
        Thread.Sleep(2000);
    }

    [When("a GET request is sent to the /Tiers/{t.id} endpoint")]
    public async Task WhenAGETRequestIsSentToTheTiersIdEndpoint()
    {
        _tierDetailsResponse = await _client.Tiers.GetTier(_tierId);
    }

    [Then("the response contains Tier t")]
    public void ThenTheResponseContainsATier()
    {
        _tierDetailsResponse!.Result!.Should().NotBeNull();
        _tierDetailsResponse!.Result!.Id.Should().Be(_tierId);
        _tierDetailsResponse.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_tierDetailsResponse!.Status).Should().Be(expectedStatusCode);
    }
}

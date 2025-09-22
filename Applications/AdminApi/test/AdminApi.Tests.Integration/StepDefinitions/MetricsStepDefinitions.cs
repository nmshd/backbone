using Backbone.AdminApi.Sdk.Endpoints.Metrics.Types.Responses;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Metrics")]
internal class MetricsStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<ListMetricsResponse>? _metricsResponse;

    public MetricsStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
    }

    [When("^a GET request is sent to the /Metrics endpoint$")]
    public async Task WhenAGETRequestIsSentToTheMetricsEndpoint()
    {
        _metricsResponse = await _client.Metrics.ListMetrics();
        _metricsResponse.ShouldBeASuccess();
    }

    [Then("the response contains a list of Metrics")]
    public async Task ThenTheResponseContainsAListOfMetrics()
    {
        _metricsResponse!.Result.ShouldNotBeNull();
        _metricsResponse.Result.Count.ShouldBeGreaterThan(0);
        _metricsResponse!.ContentType.ShouldStartWith("application/json");
        await _metricsResponse.ShouldComplyWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_metricsResponse!.Status).ShouldBe(expectedStatusCode);
    }
}

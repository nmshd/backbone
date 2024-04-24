using Backbone.AdminApi.Sdk.Endpoints.Metrics.Types.Responses;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.SdkStepDefinitions;

[Binding]
[Scope(Feature = "GET Metrics")]
internal class MetricsStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<ListMetricsResponse>? _metricsResponse;

    public MetricsStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
    }

    [When("a GET request is sent to the /Metrics endpoint")]
    public async Task WhenAGETRequestIsSentToTheMetricsEndpoint()
    {
        _metricsResponse = await _client.Metrics.GetAllMetrics();
        _metricsResponse.IsSuccess.Should().BeTrue();
    }

    [Then("the response contains a list of Metrics")]
    public void ThenTheResponseContainsAListOfMetrics()
    {
        _metricsResponse!.Result!.Should().NotBeNullOrEmpty();
        _metricsResponse.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_metricsResponse!.Status).Should().Be(expectedStatusCode);
    }
}

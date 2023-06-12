﻿using AdminApi.Tests.Integration.API;
using AdminApi.Tests.Integration.Extensions;
using AdminApi.Tests.Integration.Models;

namespace AdminApi.Tests.Integration.StepDefinitions;
internal class MetricsStepDefinitions : BaseStepDefinitions
{
    private readonly MetricsApi _metricsApi;
    private HttpResponse<List<MetricDTO>>? _response;

    public MetricsStepDefinitions(MetricsApi metricsApi) : base()
    {
        _metricsApi = metricsApi;
    }

    [When(@"a GET request is sent to the /Metrics endpoint")]
    public async Task WhenAGETRequestIsSentToTheMetricsEndpointAsync()
    {
        _response = await _metricsApi.GetAllMetrics(_requestConfiguration);
        _response.Should().NotBeNull();
        _response.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a list of Metrics")]
    public void ThenTheResponseContainsAListOfMetrics()
    {
        _response!.Content!.Result.Should().NotBeNull();
        _response!.Content!.Result.Should().NotBeEmpty();
        _response.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }
}

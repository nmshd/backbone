﻿using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Extensions;
using AdminUi.Tests.Integration.Models;

namespace AdminUi.Tests.Integration.StepDefinitions;
internal class MetricsStepDefinitions : BaseStepDefinitions
{
    private readonly MetricsApi _metricsApi;
    private HttpResponse<List<MetricDTO>>? _response;

    public MetricsStepDefinitions(MetricsApi metricsApi)
    {
        _metricsApi = metricsApi;
    }

    [When(@"a GET request is sent to the /Metrics endpoint")]
    public async Task WhenAGETRequestIsSentToTheMetricsEndpoint()
    {
        _response = await _metricsApi.GetAllMetrics(_requestConfiguration);
        _response.Should().NotBeNull();
        _response.Content.Should().NotBeNull();
    }

    [Then(@"the response contains a list of Metrics")]
    public void ThenTheResponseContainsAListOfMetrics()
    {
        _response!.Content.Result.Should().NotBeNullOrEmpty();
        _response.AssertContentCompliesWithSchema();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }
}

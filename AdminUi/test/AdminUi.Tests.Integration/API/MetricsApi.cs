﻿using AdminUi.Tests.Integration.Configuration;
using AdminUi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace AdminUi.Tests.Integration.API;
public class MetricsApi : BaseApi
{
    public MetricsApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory) : base(httpConfiguration, factory) { }

    public async Task<HttpResponse<List<MetricDTO>>> GetAllMetrics(RequestConfiguration requestConfiguration)
    {
        return await Get<List<MetricDTO>>("/Metrics", requestConfiguration);
    }
}

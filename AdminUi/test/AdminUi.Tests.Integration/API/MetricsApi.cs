﻿using Backbone.AdminUi.Tests.Integration.Configuration;
using Backbone.AdminUi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace Backbone.AdminUi.Tests.Integration.API;
internal class MetricsApi : BaseApi
{
    public MetricsApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory) : base(httpConfiguration, factory) { }

    internal async Task<HttpResponse<List<MetricDTO>>> GetAllMetrics(RequestConfiguration requestConfiguration)
    {
        return await Get<List<MetricDTO>>("/Metrics", requestConfiguration);
    }
}

using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Common.Types;
using Backbone.AdminApi.Sdk.Endpoints.Metrics.Types.Responses;

namespace Backbone.AdminApi.Sdk.Endpoints.Metrics;

public class MetricsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<AdminApiResponse<List<Metric>>> GetAllMetrics() => await _client.Get<List<Metric>>("Metrics");
}

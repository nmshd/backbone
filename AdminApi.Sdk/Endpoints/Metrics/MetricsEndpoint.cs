using Backbone.AdminApi.Sdk.Endpoints.Metrics.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Metrics;

public class MetricsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<ListMetricsResponse>> GetAllMetrics() => await _client.Get<ListMetricsResponse>("Metrics");
}

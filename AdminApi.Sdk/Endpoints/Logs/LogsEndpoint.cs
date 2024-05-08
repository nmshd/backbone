using Backbone.AdminApi.Sdk.Endpoints.Logs.Types.Requests;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Logs;

public class LogsEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<EmptyResponse>> CreateLog(LogRequest request) => await _client.Post<EmptyResponse>($"api/{API_VERSION}/Logs", request);
}

using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Common.Types;
using Backbone.AdminApi.Sdk.Endpoints.Logs.Types.Requests;

namespace Backbone.AdminApi.Sdk.Endpoints.Logs;

public class LogsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<AdminApiResponse<EmptyResponse>> CreateLog(LogRequest request) => await _client.Post<EmptyResponse>("Logs", request);
}

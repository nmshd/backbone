using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns;

public class SyncRunsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<StartSyncRunResponse>> StartSyncRun(StartSyncRunRequest request, ushort supportedDatawalletVersion) => await _client
        .Request<StartSyncRunResponse>(HttpMethod.Post, "SyncRuns")
        .Authenticate()
        .WithJson(request)
        .AddExtraHeader("X-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
        .Execute();

    public async Task<ApiResponse<SyncRun>> GetSyncRun(string id) => await _client.Get<SyncRun>($"SyncRuns/{id}");

    public async Task<ApiResponse<ListExternalEventsResponse>> ListExternalEventsOfSyncRun(string id, PaginationFilter? pagination = null)
        => await _client.Get<ListExternalEventsResponse>($"SyncRuns/{id}/ExternalEvents", null, pagination);

    public async Task<ApiResponse<FinalizeExternalEventSyncResponse>> FinalizeExternalEventSync(string id, FinalizeExternalEventSyncRequest request)
        => await _client.Put<FinalizeExternalEventSyncResponse>($"SyncRuns/{id}/FinalizeExternalEventSync", request);

    public async Task<ApiResponse<FinalizeDatawalletVersionUpgradeResponse>> FinalizeDatawalletVersionUpgrade(string id, FinalizeDatawalletVersionUpgradeRequest request)
        => await _client.Put<FinalizeDatawalletVersionUpgradeResponse>($"SyncRuns/{id}/FinalizeDatawalletVersionUpgrade", request);

    public async Task<ApiResponse<RefreshExpirationTimeResponse>> RefreshExpirationTime(string id)
        => await _client.Put<RefreshExpirationTimeResponse>($"SyncRuns/{id}/RefreshExpirationTime");
}

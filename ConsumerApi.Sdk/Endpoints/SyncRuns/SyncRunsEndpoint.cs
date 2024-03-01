using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns;

public class SyncRunsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<StartSyncRunResponse>> StartSyncRun(StartSyncRunRequest request, ushort supportedDatawalletVersion) => await _client
        .Request<StartSyncRunResponse>(HttpMethod.Post, "SyncRuns")
        .Authenticate()
        .WithJson(request)
        .AddExtraHeader("X-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
        .Execute();

    public async Task<ConsumerApiResponse<SyncRun>> GetSyncRun(string id) => await _client.Get<SyncRun>($"SyncRuns/{id}");

    public async Task<ConsumerApiResponse<List<ExternalEvent>>> GetExternalEventsOfSyncRun(string id, PaginationFilter? pagination = null)
        => await _client.Get<List<ExternalEvent>>($"SyncRuns/{id}/ExternalEvents", null, pagination);

    public async Task<ConsumerApiResponse<FinalizeExternalEventSyncResponse>> FinalizeExternalEventSync(string id, FinalizeExternalEventSyncRequest request)
        => await _client.Put<FinalizeExternalEventSyncResponse>($"SyncRuns/{id}/FinalizeExternalEventSync", request);

    public async Task<ConsumerApiResponse<FinalizeDatawalletVersionUpgradeResponse>> FinalizeDatawalletVersionUpgrade(string id, FinalizeDatawalletVersionUpgradeRequest request)
        => await _client.Put<FinalizeDatawalletVersionUpgradeResponse>($"SyncRuns/{id}/FinalizeDatawalletVersionUpgrade", request);

    public async Task<ConsumerApiResponse<RefreshExpirationTimeResponse>> RefreshExpirationTime(string id)
        => await _client.Put<RefreshExpirationTimeResponse>($"SyncRuns/{id}/RefreshExpirationTime");
}

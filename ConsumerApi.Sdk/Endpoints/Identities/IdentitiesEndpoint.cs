﻿using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities;

public class IdentitiesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<CreateIdentityResponse>> CreateIdentity(CreateIdentityRequest request)
        => await _client.PostUnauthenticated<CreateIdentityResponse>("Identities", request);

    public async Task<ApiResponse<StartDeletionProcessResponse>> StartDeletionProcess()
        => await _client.Post<StartDeletionProcessResponse>("Identities/Self/DeletionProcesses");

    public async Task<ApiResponse<ListDeletionProcessesResponse>> ListDeletionProcesses()
        => await _client.Get<ListDeletionProcessesResponse>("Identities/Self/DeletionProcesses");

    public async Task<ApiResponse<IdentityDeletionProcess>> GetDeletionProcess(string id)
        => await _client.Get<IdentityDeletionProcess>($"Identities/Self/DeletionProcesses/{id}");

    public async Task<ApiResponse<ApproveDeletionProcessResponse>> ApproveDeletionProcess(string id)
        => await _client.Put<ApproveDeletionProcessResponse>($"Identities/Self/DeletionProcesses/{id}/Approve");
}

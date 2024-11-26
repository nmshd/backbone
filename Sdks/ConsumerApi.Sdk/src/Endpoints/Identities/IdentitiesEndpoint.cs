using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities;

public class IdentitiesEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<CreateIdentityResponse>> CreateIdentity(CreateIdentityRequest request)
    {
        return await _client.PostUnauthenticated<CreateIdentityResponse>($"api/{API_VERSION}/Identities", request);
    }

    public async Task<ApiResponse<StartDeletionProcessResponse>> StartDeletionProcess()
    {
        return await _client.Post<StartDeletionProcessResponse>($"api/{API_VERSION}/Identities/Self/DeletionProcesses");
    }

    public async Task<ApiResponse<ListDeletionProcessesResponse>> ListDeletionProcesses()
    {
        return await _client.Get<ListDeletionProcessesResponse>($"api/{API_VERSION}/Identities/Self/DeletionProcesses");
    }

    public async Task<ApiResponse<IdentityDeletionProcess>> GetDeletionProcess(string id)
    {
        return await _client.Get<IdentityDeletionProcess>($"api/{API_VERSION}/Identities/Self/DeletionProcesses/{id}");
    }

    public async Task<ApiResponse<ApproveDeletionProcessResponse>> ApproveDeletionProcess(string id)
    {
        return await _client.Put<ApproveDeletionProcessResponse>($"api/{API_VERSION}/Identities/Self/DeletionProcesses/{id}/Approve");
    }

    public async Task<ApiResponse<CancelDeletionProcessResponse>> CancelDeletionProcess(string id)
    {
        return await _client.Put<CancelDeletionProcessResponse>($"api/{API_VERSION}/Identities/Self/DeletionProcesses/{id}/Cancel");
    }

    public async Task<ApiResponse<IsDeletedResponse>> IsDeleted(string username)
    {
        return await _client.GetUnauthenticated<IsDeletedResponse>($"api/{API_VERSION}/Identities/IsDeleted", new Dictionary<string, string> { { "username", username } });
    }
}

using Backbone.AdminApi.Sdk.Endpoints.Identities.Types;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Identities;

public class IdentitiesEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<IndividualQuota>> CreateIndividualQuota(string address, CreateQuotaForIdentityRequest request)
    {
        return await _client.Post<IndividualQuota>($"api/{API_VERSION}/Identities/{address}/Quotas", request);
    }

    public async Task<ApiResponse<EmptyResponse>> DeleteIndividualQuota(string address, string quotaId)
    {
        return await _client.Delete<EmptyResponse>($"api/{API_VERSION}/Identities/{address}/Quotas/{quotaId}");
    }

    public async Task<ApiResponse<ListIdentitiesResponse>> ListIdentities()
    {
        return await _client.Request<ListIdentitiesResponse>(HttpMethod.Get, "odata/Identities")
            .Authenticate()
            .ExecuteOData();
    }

    public async Task<ApiResponse<ListIdentityDeletionProcessAuditLogsResponse>?> ListIdentityDeletionProcessAuditLogs(string address)
    {
        return await _client.Get<ListIdentityDeletionProcessAuditLogsResponse>($"api/{API_VERSION}/Identities/{address}/DeletionProcesses/AuditLogs");
    }

    public async Task<ApiResponse<GetIdentityResponse>> GetIdentity(string address)
    {
        return await _client.Get<GetIdentityResponse>($"api/{API_VERSION}/Identities/{address}");
    }

    public async Task<ApiResponse<EmptyResponse>> UpdateIdentityTier(string address, UpdateIdentityTierRequest request)
    {
        return await _client.Put<EmptyResponse>($"api/{API_VERSION}/Identities/{address}", request);
    }

    public async Task<ApiResponse<CreateIdentityResponse>> CreateIdentity(CreateIdentityRequest request)
    {
        return await _client.Post<CreateIdentityResponse>($"api/{API_VERSION}/Identities", request);
    }
}

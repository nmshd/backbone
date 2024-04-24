using Backbone.AdminApi.Sdk.Endpoints.Identities.Types;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Identities;

public class IdentitiesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<IndividualQuota>> CreateIndividualQuota(string address, CreateQuotaForIdentityRequest request)
        => await _client.Post<IndividualQuota>($"Identities/{address}/Quotas", request);

    public async Task<ApiResponse<EmptyResponse>> DeleteIndividualQuota(string address, string quotaId)
        => await _client.Delete<EmptyResponse>($"Identities/{address}/Quotas/{quotaId}");

    public async Task<ApiResponse<ListIdentitiesResponse>> ListIdentities() => await _client.Request<ListIdentitiesResponse>(HttpMethod.Get, "Identities")
        .Authenticate()
        .ExecuteOData();

    public async Task<ApiResponse<GetIdentityResponse>> GetIdentity(string address) => await _client.Get<GetIdentityResponse>($"Identities/{address}");

    public async Task<ApiResponse<EmptyResponse>> UpdateIdentityTier(string address, UpdateIdentityTierRequest request)
        => await _client.Put<EmptyResponse>($"Identities/{address}", request);

    public async Task<ApiResponse<CreateIdentityResponse>> CreateIdentity(CreateIdentityRequest request)
        => await _client.Post<CreateIdentityResponse>("Identities", request);

    public async Task<ApiResponse<StartDeletionProcessAsSupportResponse>> StartDeletionProcess(string address)
        => await _client.Post<StartDeletionProcessAsSupportResponse>($"Identities/{address}/DeletionProcesses");
}

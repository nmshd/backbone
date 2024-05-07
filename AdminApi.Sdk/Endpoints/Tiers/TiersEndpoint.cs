using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Tiers;

public class TiersEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<ListTiersResponse>> ListTiers() => await _client.Get<ListTiersResponse>($"api/{API_VERSION}/Tiers");

    public async Task<ApiResponse<TierDetails>> GetTier(string id) => await _client.Get<TierDetails>($"api/{API_VERSION}/Tiers/{id}");

    public async Task<ApiResponse<Tier>> CreateTier(CreateTierRequest request) => await _client.Post<Tier>($"api/{API_VERSION}/Tiers", request);

    public async Task<ApiResponse<EmptyResponse>> DeleteTier(string id) => await _client.Delete<EmptyResponse>($"api/{API_VERSION}/Tiers/{id}");

    public async Task<ApiResponse<TierQuotaDefinition>> AddTierQuota(string id, CreateQuotaForTierRequest request)
        => await _client.Post<TierQuotaDefinition>($"api/{API_VERSION}/Tiers/{id}/Quotas", request);

    public async Task<ApiResponse<EmptyResponse>> DeleteTierQuota(string id, string quotaId)
        => await _client.Delete<EmptyResponse>($"api/{API_VERSION}/Tiers/{id}/Quotas/{quotaId}");
}

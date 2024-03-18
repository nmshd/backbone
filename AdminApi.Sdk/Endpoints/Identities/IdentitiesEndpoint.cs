using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Common.Types;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;

namespace Backbone.AdminApi.Sdk.Endpoints.Identities;

public class IdentitiesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<AdminApiResponse<IndividualQuota>> CreateIndividualQuota(string address, CreateQuotaForIdentityRequest request)
        => await _client.Post<IndividualQuota>($"Identities/{address}/Quotas", request);

    public async Task<AdminApiResponse<EmptyResponse>> DeleteIndividualQuota(string address, string quotaId)
        => await _client.Delete<EmptyResponse>($"Identities/{address}/Quotas/{quotaId}");

    public async Task<AdminApiResponse<GetIdentityResponse>> GetIdentity(string address) => await _client.Get<GetIdentityResponse>($"Identities/{address}");

    public async Task<AdminApiResponse<EmptyResponse>> UpdateIdentityTier(string address, UpdateIdentityTierRequest request)
        => await _client.Put<EmptyResponse>($"Identities/{address}", request);

    public async Task<AdminApiResponse<CreateIdentityResponse>> CreateIdentity(CreateIdentityRequest request)
        => await _client.Post<CreateIdentityResponse>("Identities", request);

    public async Task<AdminApiResponse<StartDeletionProcessAsSupportResponse>> StartDeletionProcessAsSupport(string address)
        => await _client.Post<StartDeletionProcessAsSupportResponse>($"Identities/{address}/DeletionProcess");
}

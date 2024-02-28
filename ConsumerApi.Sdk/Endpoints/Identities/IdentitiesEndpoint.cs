using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities;

public class IdentitiesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<CreateIdentityResponse>> CreateIdentity(CreateIdentityRequest request)
        => await _client.PostUnauthenticated<CreateIdentityResponse>("Identities", request);

    public async Task<ConsumerApiResponse<StartDeletionProcessResponse>> StartDeletionProcess()
        => await _client.Post<StartDeletionProcessResponse>("Identities/Self/DeletionProcesses");

    public async Task<ConsumerApiResponse<List<IdentityDeletionProcess>>> ListDeletionProcesses()
        => await _client.Get<List<IdentityDeletionProcess>>("Identities/Self/DeletionProcesses");

    public async Task<ConsumerApiResponse<IdentityDeletionProcess>> GetDeletionProcess(string id)
        => await _client.Get<IdentityDeletionProcess>($"Identities/Self/DeletionProcesses/{id}");

    public async Task<ConsumerApiResponse<ApproveDeletionProcessResponse>> ApproveDeletionProcess(string id)
        => await _client.Put<ApproveDeletionProcessResponse>($"Identities/Self/DeletionProcesses/{id}/Approve");
}

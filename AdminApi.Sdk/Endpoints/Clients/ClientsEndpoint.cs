using Backbone.AdminApi.Sdk.Endpoints.Clients.Types;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Responses;
using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Clients;

public class ClientsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<AdminApiResponse<ListClientsResponse>> GetAllClients() => await _client.Get<ListClientsResponse>("Clients");

    public async Task<AdminApiResponse<ClientInfo>> GetClient(string id) => await _client.Get<ClientInfo>($"Clients/{id}");

    public async Task<AdminApiResponse<CreateClientResponse>> CreateClient(CreateClientRequest request)
        => await _client.Post<CreateClientResponse>("Clients", request);

    public async Task<AdminApiResponse<ClientInfo>> ChangeClientSecret(string id, ChangeClientSecretRequest request)
        => await _client.Patch<ClientInfo>($"Clients/{id}/ChangeSecret", request);

    public async Task<AdminApiResponse<ClientInfo>> UpdateClient(string id, UpdateClientRequest request)
        => await _client.Put<ClientInfo>($"Clients/{id}", request);

    public async Task<AdminApiResponse<EmptyResponse>> DeleteClient(string id) => await _client.Delete<EmptyResponse>($"Clients/{id}");
}

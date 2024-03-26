using Backbone.AdminApi.Sdk.Endpoints.Clients.Types;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Clients;

public class ClientsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<ListClientsResponse>> GetAllClients() => await _client.Get<ListClientsResponse>("Clients");

    public async Task<ApiResponse<ClientInfo>> GetClient(string id) => await _client.Get<ClientInfo>($"Clients/{id}");

    public async Task<ApiResponse<CreateClientResponse>> CreateClient(CreateClientRequest request)
        => await _client.Post<CreateClientResponse>("Clients", request);

    public async Task<ApiResponse<ClientInfo>> ChangeClientSecret(string id, ChangeClientSecretRequest request)
        => await _client.Patch<ClientInfo>($"Clients/{id}/ChangeSecret", request);

    public async Task<ApiResponse<ClientInfo>> UpdateClient(string id, UpdateClientRequest request)
        => await _client.Put<ClientInfo>($"Clients/{id}", request);

    public async Task<ApiResponse<EmptyResponse>> DeleteClient(string id) => await _client.Delete<EmptyResponse>($"Clients/{id}");
}

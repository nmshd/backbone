using Backbone.AdminApi.Sdk.Endpoints.Clients.Types;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Clients;

public class ClientsEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<ListClientsResponse>> ListClients()
    {
        return await _client.Get<ListClientsResponse>($"api/{API_VERSION}/Clients");
    }

    public async Task<ApiResponse<ClientInfo>> GetClient(string id)
    {
        return await _client.Get<ClientInfo>($"api/{API_VERSION}/Clients/{id}");
    }

    public async Task<ApiResponse<CreateClientResponse>> CreateClient(CreateClientRequest request)
    {
        return await _client.Post<CreateClientResponse>($"api/{API_VERSION}/Clients", request);
    }

    public async Task<ApiResponse<ChangeClientSecretResponse>> ChangeClientSecret(string id, ChangeClientSecretRequest request)
    {
        return await _client.Patch<ChangeClientSecretResponse>($"api/{API_VERSION}/Clients/{id}/ChangeSecret", request);
    }

    public async Task<ApiResponse<UpdateClientResponse>> UpdateClient(string id, UpdateClientRequest request)
    {
        return await _client.Put<UpdateClientResponse>($"api/{API_VERSION}/Clients/{id}", request);
    }

    public async Task<ApiResponse<EmptyResponse>> DeleteClient(string id)
    {
        return await _client.Delete<EmptyResponse>($"api/{API_VERSION}/Clients/{id}");
    }
}

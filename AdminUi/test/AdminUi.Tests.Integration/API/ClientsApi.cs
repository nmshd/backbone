using AdminUi.Tests.Integration.Configuration;
using AdminUi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace AdminUi.Tests.Integration.API;
public class ClientsApi : BaseApi
{
    public ClientsApi(IOptions<HttpClientOptions> httpConfiguration) : base(httpConfiguration) { }

    public async Task<HttpResponse<List<ClientDTO>>> GetAllClients(RequestConfiguration requestConfiguration)
    {
        return await Get<List<ClientDTO>>("/Clients", requestConfiguration);
    }

    public async Task<HttpResponse<ClientDTO>> GetClient(string clientId, RequestConfiguration requestConfiguration)
    {
        return await Get<ClientDTO>($"/Clients/{clientId}", requestConfiguration);
    }

    public async Task<HttpResponse> DeleteClient(string clientId, RequestConfiguration requestConfiguration)
    {
        return await Delete($"/Clients/{clientId}", requestConfiguration);
    }

    public async Task<HttpResponse<CreateClientResponse>> CreateClient(RequestConfiguration requestConfiguration)
    {
        return await Post<CreateClientResponse>($"/Clients", requestConfiguration);
    }

    public async Task<HttpResponse<ChangeClientSecretResponse>> ChangeClientSecret(string clientId, RequestConfiguration requestConfiguration)
    {
        return await Patch<ChangeClientSecretResponse>($"/Clients/{clientId}/ChangeSecret", requestConfiguration);
    }

    public async Task<HttpResponse<UpdateClientResponse>> UpdateClient(string clientId, RequestConfiguration requestConfiguration)
    {
        return await Patch<UpdateClientResponse>($"/Clients/{clientId}", requestConfiguration);
    }
}

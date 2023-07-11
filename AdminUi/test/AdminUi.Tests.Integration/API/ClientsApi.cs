using AdminUi.Tests.Integration.Models;
using RestSharp;

namespace AdminUi.Tests.Integration.API;
public class ClientsApi : BaseApi
{
    public ClientsApi(RestClient client, string apiKey) : base(client, apiKey) { }

    public async Task<HttpResponse<List<ClientDTO>>> GetAllClients(RequestConfiguration requestConfiguration)
    {
        return await Get<List<ClientDTO>>("/Clients", requestConfiguration);
    }

    public async Task<HttpResponse<List<ClientDTO>>> DeleteClient(string clientId, RequestConfiguration requestConfiguration)
    {
        return await Delete<List<ClientDTO>>($"/Clients/{clientId}", requestConfiguration);
    }

    public async Task<HttpResponse<ClientDTO>> CreateClient(RequestConfiguration requestConfiguration)
    {
        return await Post<ClientDTO>($"/Clients", requestConfiguration);
    }
}

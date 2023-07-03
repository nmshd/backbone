using AdminApi.Tests.Integration.Models;
using RestSharp;

namespace AdminApi.Tests.Integration.API;
public class ClientsApi : BaseApi
{
    public ClientsApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<List<ClientDTO>>> GetAllClients(RequestConfiguration requestConfiguration)
    {
        return await Get<List<ClientDTO>>("/Clients", requestConfiguration);
    }
}

using Backbone.AdminUi.Tests.Integration.Configuration;
using Backbone.AdminUi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace Backbone.AdminUi.Tests.Integration.API;
internal class ClientsApi : BaseApi
{
    public ClientsApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory) : base(httpConfiguration, factory) { }

    internal async Task<HttpResponse<List<ClientOverviewDTO>>> GetAllClients(RequestConfiguration requestConfiguration)
    {
        return await Get<List<ClientOverviewDTO>>("/Clients", requestConfiguration);
    }

    internal async Task<HttpResponse<ClientDTO>> GetClient(string clientId, RequestConfiguration requestConfiguration)
    {
        return await Get<ClientDTO>($"/Clients/{clientId}", requestConfiguration);
    }

    internal async Task<HttpResponse> DeleteClient(string clientId, RequestConfiguration requestConfiguration)
    {
        return await Delete($"/Clients/{clientId}", requestConfiguration);
    }

    internal async Task<HttpResponse<CreateClientResponse>> CreateClient(RequestConfiguration requestConfiguration)
    {
        return await Post<CreateClientResponse>($"/Clients", requestConfiguration);
    }

    internal async Task<HttpResponse<ChangeClientSecretResponse>> ChangeClientSecret(string clientId, RequestConfiguration requestConfiguration)
    {
        return await Patch<ChangeClientSecretResponse>($"/Clients/{clientId}/ChangeSecret", requestConfiguration);
    }

    internal async Task<HttpResponse<UpdateClientResponse>> UpdateClient(string clientId, RequestConfiguration requestConfiguration)
    {
        return await Put<UpdateClientResponse>($"/Clients/{clientId}", requestConfiguration);
    }
}

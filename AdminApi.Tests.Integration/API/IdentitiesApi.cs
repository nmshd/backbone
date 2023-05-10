using AdminApi.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace AdminApi.Tests.Integration.API;

public class IdentitiesApi : BaseApi
{
    public IdentitiesApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<List<IdentitySummaryDTO>>> GetIdentities(RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<List<IdentitySummaryDTO>>(Method.Get, new PathString(ROUTE_PREFIX).Add($"/Identities").ToString(), requestConfiguration);
    }
}

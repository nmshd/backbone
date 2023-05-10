using AdminApi.Tests.Integration.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace AdminApi.Tests.Integration.API;

public class TiersApi : BaseApi
{
    public TiersApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<List<TierDTO>>> GetTiers(RequestConfiguration requestConfiguration)
    {
        return await ExecuteRequest<List<TierDTO>>(Method.Get, new PathString(ROUTE_PREFIX).Add($"/Tiers").ToString(), requestConfiguration);
    }
}

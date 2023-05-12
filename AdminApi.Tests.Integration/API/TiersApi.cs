using AdminApi.Tests.Integration.Models;
using RestSharp;

namespace AdminApi.Tests.Integration.API;

public class TiersApi : BaseApi
{
    public TiersApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<List<TierDTO>>> GetTiers(RequestConfiguration requestConfiguration)
    {
        return await Get<List<TierDTO>>("/Tiers", requestConfiguration);
    }
}

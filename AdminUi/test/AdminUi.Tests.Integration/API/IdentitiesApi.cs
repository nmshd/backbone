using AdminUi.Tests.Integration.Models;
using RestSharp;

namespace AdminUi.Tests.Integration.API;

public class IdentitiesApi : BaseApi
{
    public IdentitiesApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<List<IdentitySummaryDTO>>> GetIdentities(RequestConfiguration requestConfiguration)
    {
        return await Get<List<IdentitySummaryDTO>>("/Identities", requestConfiguration);
    }
}

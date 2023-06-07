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

    public async Task<HttpResponse<TierQuotaDTO>> CreateTierQuota(RequestConfiguration requestConfiguration, string tierId)
    {
        return await Post<TierQuotaDTO>($"/Tiers/{tierId}/Quotas", requestConfiguration);
    }

    public async Task<HttpResponse<TierDTO>> CreateTier(RequestConfiguration requestConfiguration)
    {
        return await Post<TierDTO>($"/Tiers", requestConfiguration);
    }
}

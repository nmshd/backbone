using AdminUi.Tests.Integration.Configuration;
using AdminUi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace AdminUi.Tests.Integration.API;

public class TiersApi : BaseApi
{
    public TiersApi(IOptions<HttpClientOptions> httpConfiguration) : base(httpConfiguration) { }

    public async Task<HttpResponse<List<TierDTO>>> GetTiers(RequestConfiguration requestConfiguration)
    {
        return await Get<List<TierDTO>>("/Tiers", requestConfiguration);
    }

    public async Task<HttpResponse<TierDetailsDTO>> GetTierById(RequestConfiguration requestConfiguration, string tierId)
    {
        return await Get<TierDetailsDTO>($"/Tiers/{tierId}", requestConfiguration);
    }

    public async Task<HttpResponse<TierQuotaDTO>> CreateTierQuota(RequestConfiguration requestConfiguration, string tierId)
    {
        return await Post<TierQuotaDTO>($"/Tiers/{tierId}/Quotas", requestConfiguration);
    }

    public async Task<HttpResponse> DeleteTierQuota(string tierId, string tierQuotaDefinitionId, RequestConfiguration requestConfiguration)
    {
        return await Delete($"/Tiers/{tierId}/Quotas/{tierQuotaDefinitionId}", requestConfiguration);
    }

    public async Task<HttpResponse<TierDTO>> CreateTier(RequestConfiguration requestConfiguration)
    {
        return await Post<TierDTO>("/Tiers", requestConfiguration);
    }

    public async Task<HttpResponse> DeleteTier(RequestConfiguration requestConfiguration, string tierId)
    {
        return await Delete($"/Tiers/{tierId}", requestConfiguration);
    }
}

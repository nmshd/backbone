using Backbone.AdminUi.Tests.Integration.Configuration;
using Backbone.AdminUi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace Backbone.AdminUi.Tests.Integration.API;

public class TiersApi : BaseApi
{
    public TiersApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory) : base(httpConfiguration, factory) { }

    public async Task<HttpResponse<List<TierOverviewDTO>>> GetTiers(RequestConfiguration requestConfiguration)
    {
        return await Get<List<TierOverviewDTO>>("/Tiers", requestConfiguration);
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

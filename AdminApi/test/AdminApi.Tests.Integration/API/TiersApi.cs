using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.API;

internal class TiersApi : BaseApi
{
    public TiersApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory) : base(httpConfiguration, factory) { }

    internal async Task<HttpResponse<List<TierOverviewDTO>>> GetTiers(RequestConfiguration requestConfiguration)
    {
        return await Get<List<TierOverviewDTO>>("/Tiers", requestConfiguration);
    }

    internal async Task<HttpResponse<TierDetailsDTO>> GetTierById(RequestConfiguration requestConfiguration, string tierId)
    {
        return await Get<TierDetailsDTO>($"/Tiers/{tierId}", requestConfiguration);
    }

    internal async Task<HttpResponse<TierQuotaDTO>> CreateTierQuota(RequestConfiguration requestConfiguration, string tierId)
    {
        return await Post<TierQuotaDTO>($"/Tiers/{tierId}/Quotas", requestConfiguration);
    }

    internal async Task<HttpResponse> DeleteTierQuota(string tierId, string tierQuotaDefinitionId, RequestConfiguration requestConfiguration)
    {
        return await Delete($"/Tiers/{tierId}/Quotas/{tierQuotaDefinitionId}", requestConfiguration);
    }

    internal async Task<HttpResponse<TierDTO>> CreateTier(RequestConfiguration requestConfiguration)
    {
        return await Post<TierDTO>("/Tiers", requestConfiguration);
    }

    internal async Task<HttpResponse> DeleteTier(RequestConfiguration requestConfiguration, string tierId)
    {
        return await Delete($"/Tiers/{tierId}", requestConfiguration);
    }
}

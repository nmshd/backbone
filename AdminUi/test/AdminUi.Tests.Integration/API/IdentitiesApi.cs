using Backbone.AdminUi.Tests.Integration.Configuration;
using Backbone.AdminUi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace Backbone.AdminUi.Tests.Integration.API;

internal class IdentitiesApi : BaseApi
{
    public IdentitiesApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory) : base(httpConfiguration, factory) { }

    internal async Task<HttpResponse<IdentitySummaryDTO>> GetIdentityByAddress(RequestConfiguration requestConfiguration, string identityAddress)
    {
        return await Get<IdentitySummaryDTO>($"/Identities/{identityAddress}", requestConfiguration);
    }

    internal async Task<HttpResponse<IndividualQuotaDTO>> CreateIndividualQuota(RequestConfiguration requestConfiguration, string identityAddress)
    {
        return await Post<IndividualQuotaDTO>($"/Identities/{identityAddress}/Quotas", requestConfiguration);
    }

    internal async Task<HttpResponse> DeleteIndividualQuota(string identityAddress, string individualQuotaId, RequestConfiguration requestConfiguration)
    {
        return await Delete($"/Identities/{identityAddress}/Quotas/{individualQuotaId}", requestConfiguration);
    }

    internal async Task<ODataResponse<List<IdentityOverviewDTO>>?> GetIdentityOverviews(RequestConfiguration requestConfiguration)
    {
        return await GetOData<List<IdentityOverviewDTO>>("/Identities?$expand=Tier", requestConfiguration);
    }

    internal async Task<HttpResponse<StartDeletionProcessAsSupportResponse>> StartDeletionProcess(string identityAddress, RequestConfiguration requestConfiguration)
    {
        return await Post<StartDeletionProcessAsSupportResponse>($"/Identities/{identityAddress}/DeletionProcesses", requestConfiguration);
    }

    internal async Task<HttpResponse<CreateIdentityResponse>> CreateIdentity(RequestConfiguration requestConfiguration)
    {
        return await Post<CreateIdentityResponse>("/Identities", requestConfiguration);
    }
}

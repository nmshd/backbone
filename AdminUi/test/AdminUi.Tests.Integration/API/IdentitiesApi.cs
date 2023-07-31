using AdminUi.Tests.Integration.Configuration;
using AdminUi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace AdminUi.Tests.Integration.API;

public class IdentitiesApi : BaseApi
{
    public IdentitiesApi(IOptions<HttpClientOptions> httpConfiguration) : base(httpConfiguration) { }

    public async Task<HttpResponse<List<IdentitySummaryDTO>>> GetIdentities(RequestConfiguration requestConfiguration)
    {
        return await Get<List<IdentitySummaryDTO>>("/Identities", requestConfiguration);
    }

    public async Task<HttpResponse<IdentitySummaryDTO>> GetIdentityByAddress(RequestConfiguration requestConfiguration, string identityAddress)
    {
        return await Get<IdentitySummaryDTO>($"/Identities/{identityAddress}", requestConfiguration);
    }

    public async Task<HttpResponse<IndividualQuotaDTO>> CreateIndividualQuota(RequestConfiguration requestConfiguration, string identityAddress)
    {
        return await Post<IndividualQuotaDTO>($"/Identities/{identityAddress}/Quotas", requestConfiguration);
    }

    public async Task<HttpResponse> DeleteIndividualQuota(string identityAddress, string individualQuotaId, RequestConfiguration requestConfiguration)
    {
        return await Delete($"/Identities/{identityAddress}/Quotas/{individualQuotaId}", requestConfiguration);
    }
}

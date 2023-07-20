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
}

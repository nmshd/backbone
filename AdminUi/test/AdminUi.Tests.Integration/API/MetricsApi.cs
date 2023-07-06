using AdminUi.Tests.Integration.Models;
using RestSharp;

namespace AdminUi.Tests.Integration.API;
public class MetricsApi : BaseApi
{
    public MetricsApi(RestClient client) : base(client) { }

    public async Task<HttpResponse<List<MetricDTO>>> GetAllMetrics(RequestConfiguration requestConfiguration)
    {
        return await Get<List<MetricDTO>>("/Metrics", requestConfiguration);
    }
}

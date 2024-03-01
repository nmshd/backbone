using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.API;
internal class LogsApi : BaseApi
{
    public LogsApi(IOptions<HttpClientOptions> httpConfiguration, HttpClientFactory factory) : base(httpConfiguration, factory) { }

    internal async Task<HttpResponse> CreateLog(RequestConfiguration requestConfiguration)
    {
        return await Post("/Logs", requestConfiguration);
    }
}

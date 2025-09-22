using System.Net;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public class SseServerHealthCheck : IHealthCheck
{
    private readonly HttpClient _client;

    public SseServerHealthCheck(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient(nameof(SseServerClient));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.GetAsync("health", cancellationToken);
            return result.StatusCode == HttpStatusCode.OK ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}

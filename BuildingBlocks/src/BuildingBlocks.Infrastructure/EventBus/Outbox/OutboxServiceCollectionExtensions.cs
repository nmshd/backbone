using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public static class OutboxServiceCollectionExtensions
{
    public static IServiceCollection AddOutboxPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<OutboxOptions>().Bind(configuration.GetSection("Outbox"))
            .Validate(x => x.BatchSize is > 0 and <= 1000 && x.Parallelism is > 0 and <= 64 && x.PollIntervalMilliseconds >= 10 && x.LeaseSeconds >= 15 &&
                           x.MaxAttempts > 0 && x.InitialRetrySeconds > 0 && x.MaxRetrySeconds >= x.InitialRetrySeconds, "Invalid outbox configuration.")
            .ValidateOnStart();
        services.AddSingleton<OutboxStore>();
        services.AddSingleton<OutboxDiagnostics>();
        services.AddHostedService<OutboxPublisher>();
        return services;
    }
}

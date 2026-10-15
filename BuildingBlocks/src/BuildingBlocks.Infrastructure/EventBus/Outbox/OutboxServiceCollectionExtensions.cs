using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public static class OutboxServiceCollectionExtensions
{
    public static IServiceCollection AddOutboxPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<OutboxOptions>(configuration.GetSection("Outbox").Bind);
        services.AddSingleton<OutboxStore>();
        services.AddSingleton<OutboxDiagnostics>();
        services.AddHostedService<OutboxPublisher>();
        return services;
    }
}

using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public static class RabbitMqServiceCollectionExtensions
{
    public static void AddRabbitMq(this IServiceCollection services, RabbitMqConfiguration configuration)
    {
        services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();

            var factory = new ConnectionFactory
            {
                HostName = configuration.HostName,
                Port = configuration.Port,
            };

            if (configuration.EnableSsl)
            {
                factory.Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = configuration.HostName
                };
            }

            if (!string.IsNullOrEmpty(configuration.Username))
                factory.UserName = configuration.Username;

            if (!string.IsNullOrEmpty(configuration.Password))
                factory.Password = configuration.Password;

            return new DefaultRabbitMqPersistentConnection(factory, logger);
        });

        services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
        {
            var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
            var metrics = sp.GetRequiredService<EventBusMetrics>();

            return EventBusRabbitMq.Create(rabbitMqPersistentConnection, logger, sp, configuration.ExchangeName, metrics).GetAwaiter().GetResult();
        });
    }
}

public class RabbitMqConfiguration
{
    [Required]
    public bool EnableSsl { get; set; } = true;

    [Required]
    [MinLength(1)]
    public string ExchangeName { get; set; } = "enmeshed";

    [Required]
    [MinLength(1)]
    public string HostName { get; set; } = null!;

    [Required]
    public int Port { get; set; } = 5672;

    [Required]
    [MinLength(1)]
    public string Username { get; set; } = null!;

    [Required]
    [MinLength(1)]
    public string Password { get; set; } = null!;
}

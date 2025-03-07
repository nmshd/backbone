using RabbitMQ.Client;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public interface IRabbitMqPersistentConnection
    : IDisposable
{
    bool IsConnected { get; }

    Task Connect();

    Task<IChannel> CreateChannel();
}

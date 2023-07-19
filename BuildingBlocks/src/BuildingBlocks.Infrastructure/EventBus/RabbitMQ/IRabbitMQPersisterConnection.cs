using RabbitMQ.Client;

namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public interface IRabbitMqPersistentConnection
    : IDisposable
{
    bool IsConnected { get; }

    bool TryConnect();

    IModel CreateModel();
}

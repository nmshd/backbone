using Backbone.Tooling;
using RabbitMQ.Client;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class ChannelPool : ObjectPool<IChannel>, IDisposable
{
    private readonly IRabbitMqPersistentConnection _connection;

    public ChannelPool(IRabbitMqPersistentConnection connection)
    {
        _connection = connection;
    }

    protected override async Task<IChannel> CreateObject()
    {
        return await _connection.CreateChannel();
    }

    public void Dispose()
    {
        foreach (var channel in Objects)
        {
            channel.Dispose();
        }
    }
}

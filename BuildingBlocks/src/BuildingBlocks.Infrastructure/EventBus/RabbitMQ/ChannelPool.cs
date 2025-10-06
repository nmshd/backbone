using Backbone.Tooling;
using RabbitMQ.Client;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class ChannelPool : ObjectPool<IChannel>, IDisposable
{
    private readonly IConnection _connection;

    public ChannelPool(IConnection connection)
    {
        _connection = connection;
    }

    protected override async Task<IChannel> CreateObject()
    {
        return await _connection.CreateChannelAsync();
    }

    public void Dispose()
    {
        foreach (var channel in Objects)
        {
            channel.Dispose();
        }
    }
}

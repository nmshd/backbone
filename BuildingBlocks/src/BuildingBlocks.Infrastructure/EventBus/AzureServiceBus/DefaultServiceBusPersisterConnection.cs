using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public class DefaultServiceBusPersisterConnection : IServiceBusPersisterConnection
{
    private readonly string _serviceBusConnectionString;

    private bool _disposed;
    private ServiceBusClient _topicClient;

    public DefaultServiceBusPersisterConnection(string serviceBusConnectionString)
    {
        _serviceBusConnectionString = serviceBusConnectionString;
        AdministrationClient = new ServiceBusAdministrationClient(_serviceBusConnectionString);
        _topicClient = new ServiceBusClient(_serviceBusConnectionString);
    }

    public ServiceBusClient TopicClient
    {
        get
        {
            if (_topicClient.IsClosed) _topicClient = new ServiceBusClient(_serviceBusConnectionString);

            return _topicClient;
        }
    }

    public ServiceBusAdministrationClient AdministrationClient { get; }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
#pragma warning disable CA2012
        _topicClient.DisposeAsync().GetAwaiter().GetResult();
#pragma warning restore CA2012
    }
}

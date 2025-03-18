using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;

public class AzureStorageAccountContainerClientFactory
{
    private readonly ILogger<AzureStorageAccountContainerClientFactory> _logger;
    private readonly AzureStorageAccountConfiguration _configuration;
    private readonly Dictionary<string, BlobContainerClient> _containerClients = new();

    public AzureStorageAccountContainerClientFactory(IOptions<AzureStorageAccountConfiguration> options, ILogger<AzureStorageAccountContainerClientFactory> logger)
    {
        _logger = logger;
        _configuration = options.Value;
    }

    public BlobContainerClient GetContainerClient(string containerName)
    {
        if (_containerClients.TryGetValue(containerName, out var container))
            return container;

        lock (_containerClients)
        {
            if (_containerClients.TryGetValue(containerName, out container))
                return container;

            var newContainer = CreateContainerClient(containerName);

            _containerClients.Add(containerName, newContainer);

            return newContainer;
        }
    }

    private BlobContainerClient CreateContainerClient(string containerName)
    {
        var newContainer = new BlobContainerClient(_configuration.ConnectionString, containerName);
        newContainer.CreateIfNotExists();

        try
        {
            newContainer.SetAccessPolicy(PublicAccessType.Blob);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogInformation(ex,
                "An error was thrown while trying to set the access policy on the BlobContainerClient. This error is ignored.");
        }

        return newContainer;
    }
}

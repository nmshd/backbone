using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;

public class AzureStorageAccountContainerClientFactory
{
    private readonly ILogger<AzureStorageAccountContainerClientFactory> _logger;
    private readonly AzureStorageAccountOptions _options;
    private readonly Dictionary<string, BlobContainerClient> _clients = new();

    public AzureStorageAccountContainerClientFactory(IOptions<AzureStorageAccountOptions> options, ILogger<AzureStorageAccountContainerClientFactory> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    public BlobContainerClient GetClient(string containerName)
    {
        if (_clients.TryGetValue(containerName, out var existingClient))
            return existingClient;

        var newClient = new BlobContainerClient(_options.ConnectionString, containerName);
        newClient.CreateIfNotExists();

        try
        {
            newClient.SetAccessPolicy(PublicAccessType.Blob);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogInformation(ex, "An error was thrown while trying to set the access policy on the BlobContainerClient. This error is ignored.");
        }

        _clients.Add(containerName, newClient);

        return newClient;
    }

}
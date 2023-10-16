﻿using Azure;
using Azure.Storage.Blobs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.Logging;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;

public class AzureStorageAccount : IBlobStorage, IDisposable
{
    private readonly IDictionary<BlobClient, byte[]> _changedBlobs;
    private readonly ILogger<AzureStorageAccount> _logger;
    private readonly AzureStorageAccountContainerClientFactory _containerClientFactory;
    private readonly IList<BlobClient> _removedBlobs;

    public AzureStorageAccount(ILogger<AzureStorageAccount> logger, AzureStorageAccountContainerClientFactory containerClientFactory)
    {
        _logger = logger;
        _containerClientFactory = containerClientFactory;
        _changedBlobs = new Dictionary<BlobClient, byte[]>();
        _removedBlobs = new List<BlobClient>();
    }

    public void Add(string folder, string blobId, byte[] content)
    {
        var container = _containerClientFactory.GetContainerClient(folder);
        var blob = container.GetBlobClient(blobId);
        _changedBlobs.Add(blob, content);
    }

    public void Remove(string folder, string blobId)
    {
        var container = _containerClientFactory.GetContainerClient(folder);
        var blob = container.GetBlobClient(blobId);
        _removedBlobs.Add(blob);
    }

    public async Task<byte[]> FindAsync(string folder, string blobId)
    {
        _logger.LogTrace("Reading blob with id '{blobId}'...", blobId);

        var container = _containerClientFactory.GetContainerClient(folder);
        try
        {
            var blob = container.GetBlobClient(blobId);
            var stream = new MemoryStream();

            await _logger.TraceTime(async () =>
                await blob.DownloadToAsync(stream), nameof(blob.DownloadToAsync));

            stream.Position = 0;

            _logger.LogTrace("Found blob with id '{blobId}'.", blobId);

            return stream.ToArray();
        }
        catch (Exception ex)
        {
            throw new NotFoundException(ex);
        }
    }

    public Task<IAsyncEnumerable<string>> FindAllAsync(string folder, string? prefix = null)
    {
        _logger.LogTrace("Listing all blobs...");
        var container = _containerClientFactory.GetContainerClient(folder);
        try
        {
            var blobs = container
                .GetBlobsAsync(prefix: prefix)
                .Select(storageObject => storageObject.Name);
            _logger.LogTrace("Found all blobs.");
            return Task.FromResult(blobs);
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an error listing all the blobs.", ex);
            throw;
        }
    }

    public async Task SaveAsync()
    {
        await _logger.TraceTime(UploadChangedBlobs, nameof(UploadChangedBlobs));
        await _logger.TraceTime(DeleteRemovedBlobs, nameof(DeleteRemovedBlobs));
    }

    public void Dispose()
    {
        _changedBlobs.Clear();
        _removedBlobs.Clear();
    }

    private async Task UploadChangedBlobs()
    {
        _logger.LogTrace("Uploading '{changedBlobsCount}' changed blobs...", _changedBlobs.Count);

        var changedBlobs = new Dictionary<BlobClient, byte[]>(_changedBlobs);
        foreach (var (cloudBlockBlob, bytes) in changedBlobs)
        {
            await using var memoryStream = new MemoryStream(bytes);
            try
            {
                await cloudBlockBlob.UploadAsync(memoryStream);
                _changedBlobs.Remove(cloudBlockBlob);
            }
            catch (RequestFailedException ex)
            {
                if (ex.ErrorCode == "BlobAlreadyExists") throw new BlobAlreadyExistsException(cloudBlockBlob.Name);
            }
        }

        _logger.LogTrace("Upload successful.");
    }

    private async Task DeleteRemovedBlobs()
    {
        _logger.LogTrace("Deleting '{changedBlobsCount}' blobs...", _changedBlobs.Count);

        var blobsToDelete = new List<BlobClient>(_removedBlobs);

        foreach (var cloudBlockBlob in blobsToDelete)
            try
            {
                await cloudBlockBlob.DeleteAsync();
                _removedBlobs.Remove(cloudBlockBlob);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error deleting the blob with id '{cloudBlockBlobName}'. {ex}", cloudBlockBlob.Name, ex);
                throw new NotFoundException();
            }

        _logger.LogTrace("Deletion successful.");
    }
}

using System.Net;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Google;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;

public class GoogleCloudStorage : IBlobStorage, IDisposable
{
    private readonly StorageClient _storageClient;
    private readonly List<ChangedBlob> _changedBlobs;
    private readonly IList<RemovedBlob> _removedBlobs;
    private readonly ILogger<GoogleCloudStorage> _logger;

    public GoogleCloudStorage(StorageClient storageClient, ILogger<GoogleCloudStorage> logger)
    {
        _storageClient = storageClient;
        _changedBlobs = new List<ChangedBlob>();
        _removedBlobs = new List<RemovedBlob>();
        _logger = logger;
    }

    public void Add(string folder, string blobId, byte[] content)
    {
        _changedBlobs.Add(new ChangedBlob(folder, blobId, content));
    }

    public void Remove(string folder, string blobId)
    {
        _removedBlobs.Add(new RemovedBlob(folder, blobId));
    }

    public void Dispose()
    {
        _changedBlobs.Clear();
        _removedBlobs.Clear();
    }

    public async Task<byte[]> FindAsync(string folder, string blobId)
    {
        _logger.LogTrace("Reading blob with key '{blobId}'...", blobId);

        try
        {
            var stream = new MemoryStream();

            await _logger.TraceTime(async () =>
            {
                await _storageClient.DownloadObjectAsync(folder, blobId, stream);
            }, nameof(FindAsync));

            stream.Position = 0;
            _logger.LogTrace("Found blob with key '{blobId}'.", blobId);

            return stream.ToArray();
        }
        catch (Exception ex)
        {
            EliminateNotFound(ex, blobId);
            _logger.LogError("There was an error downloading the blob with key '{blobId}'. {ex}", blobId, ex);
            throw;
        }
    }

    public Task<IAsyncEnumerable<string>> FindAllAsync(string folder, string? prefix = null)
    {
        _logger.LogTrace("Listing all blobs...");
        try
        {
            var blobs = _storageClient
                .ListObjectsAsync(folder, prefix)
                .Select(storageObject => storageObject.Name);
            _logger.LogTrace("Found all blobs.");
            return _logger.TraceTimeAsyncEnumeration(blobs, nameof(FindAllAsync));
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an error listing all the blobs.", ex);
            throw;
        }
    }

    private void EliminateNotFound(Exception ex, string blobId)
    {
        if (ex is GoogleApiException { HttpStatusCode: HttpStatusCode.NotFound })
        {
            _logger.LogError("A blob with key '{blobId}' was not found.", blobId);
            throw new NotFoundException("Blob", ex);
        }
    }

    public async Task SaveAsync()
    {
        await UploadChangedBlobs();
        await DeleteRemovedBlobs();
    }

    private async Task UploadChangedBlobs()
    {
        _logger.LogTrace("Uploading '{changedBlobsCount}' changed blobs...", _changedBlobs.Count);

        var changedBlobs = new List<ChangedBlob>(_changedBlobs);

        foreach (var blob in changedBlobs)
        {
            await EnsureKeyDoesNotExist(blob.Folder, blob.Name);

            var memoryStream = await _logger.TraceTime(async () => new MemoryStream(blob.Content), nameof(MemoryStream));

            try
            {
                _logger.LogTrace("Uploading blob with key '{blobName}'...", blob.Name);

                await _logger.TraceTime(async () =>
                {
                    await _storageClient.UploadObjectAsync(blob.Folder, blob.Name, null, memoryStream);
                }, nameof(_storageClient.UploadObjectAsync));

                _logger.LogTrace("Upload of blob with key '{blobName}' was successful.", blob.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error uploading the blob with key '{blobName}'. {ex}", blob.Name, ex);
                throw;
            }
            finally
            {
                _changedBlobs.Remove(blob);
            }
        }
    }

    private async Task EnsureKeyDoesNotExist(string folder, string key)
    {
        try
        {
            await _logger.TraceTime(async () =>
            {
                await _storageClient.GetObjectAsync(folder, key);
            }, nameof(_storageClient.GetObjectAsync));
            
            _logger.LogError("The blob with the given key already exists.");
            throw new BlobAlreadyExistsException(key);
        }
        catch (GoogleApiException ex)
        {
            if (ex.HttpStatusCode == HttpStatusCode.NotFound)
                return;

            throw;
        }
    }

    private async Task DeleteRemovedBlobs()
    {
        _logger.LogTrace("Deleting '{changedBlobsCount}' blobs...", _changedBlobs.Count);

        var blobsToDelete = new List<RemovedBlob>(_removedBlobs);

        foreach (var blob in blobsToDelete)
            try
            {
                await _logger.TraceTime(async () =>
                {
                    await _storageClient.DeleteObjectAsync(blob.Folder, blob.Name);
                }, nameof(_storageClient.DeleteObjectAsync));
                
                _removedBlobs.Remove(blob);
            }
            catch (Exception ex)
            {
                EliminateNotFound(ex, blob.Name);
                _logger.LogError("There was an error deleting the blob with key '{blobKey}'. {ex}", blob, ex);
                throw;
            }

        _logger.LogTrace("Deletion successful.");
    }

    private record ChangedBlob(string Folder, string Name, byte[] Content);
    private record RemovedBlob(string Folder, string Name);
}

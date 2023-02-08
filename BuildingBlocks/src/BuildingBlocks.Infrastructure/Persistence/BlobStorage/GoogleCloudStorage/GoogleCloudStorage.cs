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
    private readonly List<KeyValuePair<string, byte[]>> _changedBlobs;
    private readonly IList<string> _removedBlobs;
    private readonly ILogger<GoogleCloudStorage> _logger;
    private readonly string _bucketName;

    public GoogleCloudStorage(string bucketName,
        StorageClient storageClient, ILogger<GoogleCloudStorage> logger)
    {
        _bucketName = bucketName;
        _storageClient = storageClient;
        _changedBlobs = new List<KeyValuePair<string, byte[]>>();
        _removedBlobs = new List<string>();
        _logger = logger;
    }

    public void Add(string blobId, byte[] content)
    {
        _changedBlobs.Add(new KeyValuePair<string, byte[]>(blobId, content));
    }

    public void Remove(string blobId)
    {
        _removedBlobs.Add(blobId);
    }

    public void Dispose()
    {
        _changedBlobs.Clear();
        _removedBlobs.Clear();
    }

    public async Task<byte[]> FindAsync(string blobId)
    {
        _logger.LogTrace($"Reading blob with key {blobId}...");

        try
        {
            var stream = new MemoryStream();
            await _storageClient.DownloadObjectAsync(_bucketName, blobId, stream);
            stream.Position = 0;
            _logger.LogTrace($"Found blob with key {blobId}.");

            return stream.ToArray();
        }
        catch (Exception ex)
        {
            EliminateNotFound(ex, blobId);
            _logger.LogError($"There was an error downloading the blob with key {blobId}.", ex);
            throw;
        }
    }

    public Task<IAsyncEnumerable<string>> FindAllAsync(string? prefix = null)
    {
        _logger.LogTrace("Listing all blobs...");
        try
        {
            var blobs = _storageClient
                .ListObjectsAsync(_bucketName, prefix)
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

    private void EliminateNotFound(Exception ex, string blobId)
    {
        if (ex is GoogleApiException { HttpStatusCode: HttpStatusCode.NotFound })
        {
            _logger.LogError($"A blob with key {blobId} was not found.");
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
        _logger.LogTrace($"Uploading {_changedBlobs.Count} changed blobs...");

        var changedBlobs = new List<KeyValuePair<string, byte[]>>(_changedBlobs);

        foreach (var blob in changedBlobs)
        {
            await EnsureKeyDoesNotExist(blob.Key);

            await using var memoryStream = new MemoryStream(blob.Value);

            try
            {
                _logger.LogTrace($"Uploading blob with key {blob.Key}...");
                await _storageClient.UploadObjectAsync(_bucketName, blob.Key, null,
                    memoryStream);
                _logger.LogTrace($"Upload of blob with key {blob.Key} was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"There was an error uploading the blob with key {blob.Key}.", ex);
                throw;
            }
            finally
            {
                _changedBlobs.Remove(blob);
            }
        }
    }

    private async Task EnsureKeyDoesNotExist(string key)
    {
        try
        {
            await _storageClient.GetObjectAsync(_bucketName, key);
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
        _logger.LogTrace($"Deleting {_changedBlobs.Count} blobs...");

        var blobsToDelete = new List<string>(_removedBlobs);

        foreach (var blobId in blobsToDelete)
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, blobId);
                _removedBlobs.Remove(blobId);
            }
            catch (Exception ex)
            {
                EliminateNotFound(ex, blobId);
                _logger.LogError($"There was an error deleting the blob with key {blobId}.", ex);
                throw;
            }

        _logger.LogTrace("Deletion successful.");
    }
}
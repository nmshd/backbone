using System.Net;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Google;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;

public class GoogleCloudStorage : IBlobStorage, IDisposable
{
    private readonly StorageClient _storageClient;
    private readonly List<ChangedBlob> _changedBlobs;
    private readonly List<RemovedBlob> _removedBlobs;
    private readonly ILogger<GoogleCloudStorage> _logger;

    public GoogleCloudStorage(StorageClient storageClient, ILogger<GoogleCloudStorage> logger)
    {
        _storageClient = storageClient;
        _changedBlobs = [];
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

    public async Task<byte[]> GetAsync(string folder, string blobId)
    {
        _logger.LogTrace("Reading blob with key '{blobId}'...", blobId);

        try
        {
            var stream = new MemoryStream();

            await _logger.TraceTime(async () =>
                await _storageClient.DownloadObjectAsync(folder, blobId, stream), nameof(GetAsync));

            stream.Position = 0;
            _logger.LogTrace("Found blob with key '{blobId}'.", blobId);

            return stream.ToArray();
        }
        catch (Exception ex)
        {
            EliminateNotFound(ex, blobId);
            _logger.ErrorDownloadingBlobWithName(blobId, ex);
            throw;
        }
    }

    public Task<IAsyncEnumerable<string>> ListAllAsync(string folder, string? prefix = null)
    {
        _logger.LogTrace("Listing all blobs...");
        try
        {
            var blobs = _storageClient
                .ListObjectsAsync(folder, prefix)
                .Select(storageObject => storageObject.Name);
            _logger.LogTrace("Found all blobs.");
            return Task.FromResult(blobs);
        }
        catch (Exception ex)
        {
            _logger.ErrorListingAllBlobs(ex);
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
        await _logger.TraceTime(UploadChangedBlobs, nameof(UploadChangedBlobs));
        await _logger.TraceTime(DeleteRemovedBlobs, nameof(DeleteRemovedBlobs));
    }

    private async Task UploadChangedBlobs()
    {
        _logger.LogTrace("Uploading '{changedBlobsCount}' changed blobs...", _changedBlobs.Count);

        var changedBlobs = new List<ChangedBlob>(_changedBlobs);

        foreach (var blob in changedBlobs)
        {
            await EnsureKeyDoesNotExist(blob.Folder, blob.Name);

            await using var memoryStream = new MemoryStream(blob.Content);

            try
            {
                _logger.LogTrace("Uploading blob with key '{blobName}'...", blob.Name);
                await _storageClient.UploadObjectAsync(blob.Folder, blob.Name, null,
                    memoryStream);
                _logger.LogTrace("Upload of blob with key '{blobName}' was successful.", blob.Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorUploadingBlobWithName(blob.Name, ex);
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
                await _storageClient.GetObjectAsync(folder, key), nameof(_storageClient.GetObjectAsync));

            _logger.ErrorBlobWithNameExists(key);
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
                await _storageClient.DeleteObjectAsync(blob.Folder, blob.Name);
                _removedBlobs.Remove(blob);
            }
            catch (Exception ex)
            {
                EliminateNotFound(ex, blob.Name);
                _logger.ErrorDeletingBlobWithName(blob.Name, ex);
                throw;
            }

        _logger.LogTrace("Deletion successful.");
    }

    private record ChangedBlob(string Folder, string Name, byte[] Content);

    private record RemovedBlob(string Folder, string Name);
}

internal static partial class GoogleCloudStorageLogs
{
    [LoggerMessage(
        EventId = 997942,
        EventName = "GoogleCloudStorage.ErrorDownloadingBlobWithName",
        Level = LogLevel.Error,
        Message = "There was an error downloading the blob with name '{blobName}'.")]
    public static partial void ErrorDownloadingBlobWithName(this ILogger logger, string blobName, Exception ex);

    [LoggerMessage(
        EventId = 998879,
        EventName = "GoogleCloudStorage.ErrorListingAllBlobs",
        Level = LogLevel.Error,
        Message = "There was an error listing all the blobs.")]
    public static partial void ErrorListingAllBlobs(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 166344,
        EventName = "GoogleCloudStorage.ErrorUploadingBlobWithName",
        Level = LogLevel.Error,
        Message = "There was an error uploading the blob with name '{blobName}'.")]
    public static partial void ErrorUploadingBlobWithName(this ILogger logger, string blobName, Exception ex);

    [LoggerMessage(
        EventId = 358892,
        EventName = "GoogleCloudStorage.ErrorBlobWithNameExists",
        Level = LogLevel.Error,
        Message = "The blob with the name {blobName} already exists.")]
    public static partial void ErrorBlobWithNameExists(this ILogger logger, string blobName);

    [LoggerMessage(
        EventId = 304533,
        EventName = "GoogleCloudStorage.ErrorDeletingBlobWithName",
        Level = LogLevel.Error,
        Message = "There was an error downloading the blob with name '{blobName}'.")]
    public static partial void ErrorDeletingBlobWithName(this ILogger logger, string blobName, Exception ex);
}

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
                await _storageClient.DownloadObjectAsync(folder, blobId, stream), nameof(FindAsync));

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

    public Task<IAsyncEnumerable<string>> FindAllAsync(string folder, string? prefix = null)
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

            _logger.ErrorBlobWithNameExists();
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

file static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception, Exception> ERROR_DOWNLOADING_BLOB_WITH_NAME =
        LoggerMessage.Define<string, Exception>(
            LogLevel.Error,
            new EventId(997942, "GoogleCloudStorage.ErrorDownloadingBlobWithName"),
            "There was an error downloading the blob with name '{blobId}'. {ex}"
        );

    private static readonly Action<ILogger, Exception> ERROR_LISTING_ALL_BLOBS =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(998879, "GoogleCloudStorage.ErrorListingAllBlobs"),
            "There was an error listing all the blobs."
        );

    private static readonly Action<ILogger, string, Exception, Exception> ERROR_UPLOADING_BLOB_WITH_NAME =
        LoggerMessage.Define<string, Exception>(
            LogLevel.Error,
            new EventId(166344, "GoogleCloudStorage.ErrorUploadingBlobWithName"),
            "There was an error uploading the blob with name '{blobName}'. {ex}"
        );

    private static readonly Action<ILogger, Exception> ERROR_BLOB_WITH_NAME_EXISTS =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(358892, "GoogleCloudStorage.ErrorBlobWithNameExists"),
            "The blob with the given name already exists."
        );

    private static readonly Action<ILogger, string, Exception, Exception> ERROR_DELETING_BLOB_WITH_NAME =
        LoggerMessage.Define<string, Exception>(
            LogLevel.Error,
            new EventId(304533, "GoogleCloudStorage.ErrorDeletingBlobWithName"),
            "There was an error downloading the blob with name '{blobName}'. {ex}"
        );

    public static void ErrorDownloadingBlobWithName(this ILogger logger, string blobId, Exception e)
    {
        ERROR_DOWNLOADING_BLOB_WITH_NAME(logger, blobId, e, e);
    }

    public static void ErrorListingAllBlobs(this ILogger logger, Exception e)
    {
        ERROR_LISTING_ALL_BLOBS(logger, e);
    }

    public static void ErrorUploadingBlobWithName(this ILogger logger, string blobName, Exception e)
    {
        ERROR_UPLOADING_BLOB_WITH_NAME(logger, blobName, e, e);
    }

    public static void ErrorBlobWithNameExists(this ILogger logger)
    {
        ERROR_BLOB_WITH_NAME_EXISTS(logger, default!);
    }

    public static void ErrorDeletingBlobWithName(this ILogger logger, string blobName, Exception e)
    {
        ERROR_DELETING_BLOB_WITH_NAME(logger, blobName, e, e);
    }
}

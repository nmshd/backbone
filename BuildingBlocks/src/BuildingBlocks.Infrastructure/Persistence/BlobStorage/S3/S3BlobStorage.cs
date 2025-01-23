using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.S3;

public class S3BlobStorage : IBlobStorage, IDisposable
{
    private readonly AmazonS3Client _s3Client;
    private readonly List<ChangedBlob> _changedBlobs;
    private readonly IList<RemovedBlob> _removedBlobs;
    private readonly ILogger<S3BlobStorage> _logger;

    public S3BlobStorage(IOptions<S3BucketOptions> config, ILogger<S3BlobStorage> logger)
    {
        var s3Config = new AmazonS3Config
        {
            ServiceURL = config.Value.ServiceUrl,
            ForcePathStyle = true
        };

        _s3Client = new AmazonS3Client(config.Value.AccessKeyId, config.Value.SecretAccessKey, s3Config);
        _changedBlobs = [];
        _removedBlobs = [];
        _logger = logger;
    }

    public void Add(string folder, string id, byte[] content)
    {
        _changedBlobs.Add(new ChangedBlob(folder, id, content));
    }

    public void Remove(string folder, string id)
    {
        _removedBlobs.Add(new RemovedBlob(folder, id));
    }

    public void Dispose()
    {
        _changedBlobs.Clear();
        _removedBlobs.Clear();
    }

    public async Task<byte[]> FindAsync(string folder, string id)
    {
        _logger.LogTrace("Reading blob with key '{blobId}'...", id);

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = folder,
                Key = id
            };

            using var response = await _s3Client.GetObjectAsync(request);
            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);

            _logger.LogTrace("Found blob with key '{blobId}'.", id);
            return memoryStream.ToArray();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError("A blob with key '{blobId}' was not found.", id);
            throw new NotFoundException("Blob", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading blob with key '{blobId}'.", id);
            throw;
        }
    }

    public Task<IAsyncEnumerable<string>> FindAllAsync(string folder, string? prefix = null)
    {
        return Task.FromResult(FindAllBlobsAsync(folder, prefix));
    }

    private async IAsyncEnumerable<string> FindAllBlobsAsync(string folder, string? prefix)
    {
        _logger.LogTrace("Listing all blobs...");

        var request = new ListObjectsV2Request
        {
            BucketName = folder,
            Prefix = prefix ?? ""
        };

        ListObjectsV2Response response;
        do
        {
            response = await _s3Client.ListObjectsV2Async(request);

            foreach (var obj in response.S3Objects)
            {
                yield return obj.Key;
            }

            request.ContinuationToken = response.NextContinuationToken;
        } while (response.IsTruncated);

        _logger.LogTrace("Found all blobs.");
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

            using var memoryStream = new MemoryStream(blob.Content);

            try
            {
                _logger.LogTrace("Uploading blob with key '{blobName}'...", blob.Name);

                var request = new TransferUtilityUploadRequest
                {
                    InputStream = memoryStream,
                    Key = blob.Name,
                    BucketName = blob.Folder
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(request);

                _logger.LogTrace("Upload of blob with key '{blobName}' was successful.", blob.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading blob with key '{blobName}'.", blob.Name);
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
            var request = new GetObjectRequest
            {
                BucketName = folder,
                Key = key
            };

            await _s3Client.GetObjectAsync(request);

            _logger.LogError("A blob with key '{blobName}' already exists.", key);
            throw new BlobAlreadyExistsException(key);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
        }
    }

    private async Task DeleteRemovedBlobs()
    {
        _logger.LogTrace("Deleting '{removedBlobsCount}' blobs...", _removedBlobs.Count);

        var blobsToDelete = new List<RemovedBlob>(_removedBlobs);

        foreach (var blob in blobsToDelete)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = blob.Folder,
                    Key = blob.Name
                };

                await _s3Client.DeleteObjectAsync(request);

                _removedBlobs.Remove(blob);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogError("A blob with key '{blobId}' was not found.", blob.Name);
                throw new NotFoundException($"Blob with key '{blob.Name}' was not found.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blob with key '{blobName}'.", blob.Name);
                throw;
            }
        }

        _logger.LogTrace("Deletion successful.");
    }

    private record ChangedBlob(string Folder, string Name, byte[] Content);

    private record RemovedBlob(string Folder, string Name);
}

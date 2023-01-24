using Azure;
using Azure.Storage.Blobs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.Logging;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureBlobStorage
{
    public class AzureBlobStorage : IBlobStorage, IDisposable
    {
        private readonly BlobContainerClient _blobContainer;

        private readonly IDictionary<BlobClient, byte[]> _changedBlobs;
        private readonly ILogger<AzureBlobStorage> _logger;
        private readonly IList<BlobClient> _removedBlobs;

        public AzureBlobStorage(BlobContainerClient blobContainer, ILogger<AzureBlobStorage> logger)
        {
            _blobContainer = blobContainer;
            _logger = logger;
            _changedBlobs = new Dictionary<BlobClient, byte[]>();
            _removedBlobs = new List<BlobClient>();
        }

        public void Add(string blobId, byte[] content)
        {
            var blob = _blobContainer.GetBlobClient(blobId);
            _changedBlobs.Add(blob, content);
        }

        public void Remove(string blobId)
        {
            var blob = _blobContainer.GetBlobClient(blobId);
            _removedBlobs.Add(blob);
        }

        public async Task<byte[]> FindAsync(string blobId)
        {
            _logger.LogTrace($"Reading blob with id {blobId}...");

            try
            {
                var blob = _blobContainer.GetBlobClient(blobId);
                var stream = new MemoryStream();
                await blob.DownloadToAsync(stream);
                stream.Position = 0;

                _logger.LogTrace($"Found blob with id {blobId}.");

                return stream.ToArray();
            }
            catch (Exception ex)
            {
                throw new NotFoundException(ex);
            }
        }

        public async Task SaveAsync()
        {
            await UploadChangedBlobs();
            await DeleteRemovedBlobs();
        }

        public void Dispose()
        {
            _changedBlobs.Clear();
            _removedBlobs.Clear();
        }

        private async Task UploadChangedBlobs()
        {
            _logger.LogTrace($"Uploading {_changedBlobs.Count} changed blobs...");

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
            _logger.LogTrace($"Deleting {_changedBlobs.Count} blobs...");

            var blobsToDelete = new List<BlobClient>(_removedBlobs);

            foreach (var cloudBlockBlob in blobsToDelete)
                try
                {
                    await cloudBlockBlob.DeleteAsync();
                    _removedBlobs.Remove(cloudBlockBlob);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"There was an error deleting the blob with id {cloudBlockBlob.Name}.", ex);
                    throw new NotFoundException();
                }

            _logger.LogTrace("Deletion successful.");
        }
    }
}
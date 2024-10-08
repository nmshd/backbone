using System.Text;
using Backbone.Tooling.Extensions;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;

public class GoogleCloudStorageHealthCheck : IHealthCheck
{
    private const string FILE_TEXT = "healthcheck";
    private static bool? _isHealthy;

    private readonly StorageClient _storage;
    private readonly GoogleCloudStorageOptions _options;

    public GoogleCloudStorageHealthCheck(StorageClient storage, GoogleCloudStorageOptions options)
    {
        _storage = storage;
        _options = options;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!_isHealthy.HasValue)
        {
            var filename = Guid.NewGuid().ToString();
            var isUploadPossible = await IsUploadPossible(filename, cancellationToken);
            var isDownloadPossible = await IsDownloadPossible(filename, cancellationToken);
            var isDeletionPossible = await IsDeletionPossible(filename, cancellationToken);

            _isHealthy = isUploadPossible && isDownloadPossible && isDeletionPossible;
        }

        return _isHealthy.Value ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    }

    private async Task<bool> IsUploadPossible(string filename, CancellationToken cancellationToken = default)
    {
        try
        {
            await _storage.UploadObjectAsync(_options.BucketName, filename, "text/plain", new MemoryStream(FILE_TEXT.GetBytes()), null, cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> IsDownloadPossible(string filename, CancellationToken cancellationToken = default)
    {
        try
        {
            var downloadStream = new MemoryStream();
            await _storage.DownloadObjectAsync(_options.BucketName, filename, downloadStream, null, cancellationToken);
            var downloadedString = Encoding.UTF8.GetString(downloadStream.ToArray());
            return downloadedString == FILE_TEXT;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> IsDeletionPossible(string filename, CancellationToken cancellationToken = default)
    {
        try
        {
            await _storage.DeleteObjectAsync(_options.BucketName, filename, null, cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

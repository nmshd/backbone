using System.Text;
using Backbone.Tooling.Extensions;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;

public class GoogleCloudStorageHealthCheck : IHealthCheck
{
    public GoogleCloudStorageHealthCheck(StorageClient storage, GoogleCloudStorageOptions options)
    {
        _storage = storage;
        _options = options;
    }

    private readonly StorageClient _storage;
    private readonly GoogleCloudStorageOptions _options;

    private static bool? _isHealthy;
    private const string FILE_TEXT = "healthcheck";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!_isHealthy.HasValue)
        {
            try
            {
                //Upload random Blob with predefined text
                var filename = Guid.NewGuid().ToString();
                await _storage.UploadObjectAsync(_options.BucketName, filename, "text/plain", new MemoryStream(FILE_TEXT.GetBytes()), null, cancellationToken);

                //Download the Blob again and check the text
                var downloadStream = new MemoryStream();
                await _storage.DownloadObjectAsync(_options.BucketName, filename, downloadStream, null, cancellationToken);
                var downloadedString = Encoding.UTF8.GetString(downloadStream.ToArray());
                if (downloadedString != FILE_TEXT) _isHealthy = false;

                //Delete the Blob
                await _storage.DeleteObjectAsync(_options.BucketName, filename, null, cancellationToken);

                _isHealthy = true;
            }
            catch (Exception)
            {
                _isHealthy = false;
            }
        }

        return _isHealthy.Value ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    }
}

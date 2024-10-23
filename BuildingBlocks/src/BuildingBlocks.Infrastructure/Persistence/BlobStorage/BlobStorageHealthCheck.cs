using System.Text;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;

public class BlobStorageHealthCheck : IHealthCheck
{
    private const string FILE_TEXT = "healthcheck";
    private const int MAX_NUMBER_OF_TRIES = 5;

    private static bool? _isHealthy;
    private static int _numberOfTries;

    private readonly IBlobStorage _storage;
    private readonly string _rootFolderName;

    public BlobStorageHealthCheck(IBlobStorage storage, string rootFolderName)
    {
        _storage = storage;
        _rootFolderName = rootFolderName;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!_isHealthy.HasValue || (_isHealthy == false && _numberOfTries < MAX_NUMBER_OF_TRIES))
        {
            var filename = Guid.NewGuid().ToString();
            var isUploadPossible = await IsUploadPossible(filename);
            var isDownloadPossible = await IsDownloadPossible(filename);
            var isDeletionPossible = await IsDeletionPossible(filename);

            _isHealthy = isUploadPossible && isDownloadPossible && isDeletionPossible;
            _numberOfTries++;
        }

        return _isHealthy.Value ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    }

    private async Task<bool> IsUploadPossible(string filename)
    {
        try
        {
            _storage.Add(_rootFolderName, filename, FILE_TEXT.GetBytes());
            await _storage.SaveAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> IsDownloadPossible(string filename)
    {
        try
        {
            var downloadBytes = await _storage.FindAsync(_rootFolderName, filename);
            var downloadedString = Encoding.UTF8.GetString(downloadBytes);
            return downloadedString == FILE_TEXT;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> IsDeletionPossible(string filename)
    {
        try
        {
            _storage.Remove(_rootFolderName, filename);
            await _storage.SaveAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

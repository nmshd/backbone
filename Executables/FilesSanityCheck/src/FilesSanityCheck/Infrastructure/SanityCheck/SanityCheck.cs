using Backbone.FilesSanityCheck.Infrastructure.DataSource;
using Backbone.FilesSanityCheck.Infrastructure.Reporter;
using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.FilesSanityCheck.Infrastructure.SanityCheck;

public class SanityCheck
{
    private readonly IDataSource _dataSource;
    private readonly IReporter _reporter;
    private List<string> _blobIds;
    private List<FileId> _databaseIds;

    public SanityCheck(IDataSource dataSource, IReporter reporter)
    {
        _dataSource = dataSource;
        _reporter = reporter;
        _databaseIds = [];
        _blobIds = [];
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        _databaseIds = (await _dataSource.GetDatabaseIdsAsync(cancellationToken)).ToList();
        _blobIds = (await _dataSource.GetBlobIdsAsync(cancellationToken)).ToList();

        ReportOrphanedDatabaseIds();

        if (cancellationToken.IsCancellationRequested)
            return;

        ReportOrphanedBlobIds();

        _reporter.Complete();
    }

    private void ReportOrphanedDatabaseIds()
    {
        foreach (var databaseId in GetOrphanedDatabaseIds())
        {
            _reporter.ReportOrphanedDatabaseId(databaseId);
        }
    }

    private IEnumerable<FileId> GetOrphanedDatabaseIds()
    {
        Func<FileId, bool> noCorrespondingBlobIdExists = databaseId => _blobIds.All(blobId => blobId != databaseId.Value);
        return _databaseIds.Where(noCorrespondingBlobIdExists);
    }

    private void ReportOrphanedBlobIds()
    {
        foreach (var blobId in GetOrphanedBlobIds())
        {
            _reporter.ReportOrphanedBlobId(blobId);
        }
    }

    private IEnumerable<string> GetOrphanedBlobIds()
    {
        Func<string, bool> noCorrespondingDatabaseIdExists = blobId => _databaseIds.All(databaseId => databaseId.Value != blobId);
        return _blobIds.Where(noCorrespondingDatabaseIdExists);
    }
}

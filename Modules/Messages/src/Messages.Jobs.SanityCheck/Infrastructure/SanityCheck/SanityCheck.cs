﻿using Backbone.Modules.Messages.Domain.Ids;
using Messages.Jobs.SanityCheck.Infrastructure.DataSource;
using Messages.Jobs.SanityCheck.Infrastructure.Reporter;

namespace Messages.Jobs.SanityCheck.Infrastructure.SanityCheck;

public class SanityCheck
{
    private readonly IDataSource _dataSource;
    private readonly IReporter _reporter;
    private List<MessageId> _databaseIds;
    private List<string> _blobIds;

    public SanityCheck(IDataSource dataSource, IReporter reporter)
    {
        _dataSource = dataSource;
        _reporter = reporter;
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

    private IEnumerable<MessageId> GetOrphanedDatabaseIds()
    {
        Func<MessageId, bool> noCorrespondingBlobIdExists = databaseId => _blobIds.All(blobId => blobId != databaseId.StringValue);
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
        Func<string, bool> noCorrespondingDatabaseIdExists = blobId => _databaseIds.All(databaseId => databaseId.StringValue != blobId);
        return _blobIds.Where(noCorrespondingDatabaseIdExists);
    }
}


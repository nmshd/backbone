using Backbone.Modules.Synchronization.Domain.Entities;
using FluentAssertions;
using Synchronization.Jobs.SanityCheck.Tests.Infrastructure.DataSource;
using Synchronization.Jobs.SanityCheck.Tests.Infrastructure.Reporter;
using Xunit;

namespace Synchronization.Jobs.SanityCheck.Tests.Tests;

public class SanityCheckTests
{
    private readonly FakeDataSource _dataSource;
    private readonly TestReporter _reporter;
    private readonly SanityCheck.Infrastructure.SanityCheck.SanityCheck _sanityCheck;

    public SanityCheckTests()
    {
        _dataSource = new FakeDataSource();
        _reporter = new TestReporter();
        _sanityCheck = new SanityCheck.Infrastructure.SanityCheck.SanityCheck(_dataSource, _reporter);
    }

    [Fact]
    public async Task SanityCheckNoEntries()
    {
        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckConsistentEntries()
    {
        var datawalletModificationId = DatawalletModificationId.New();

        _dataSource.BlobIds.Add(datawalletModificationId);
        _dataSource.DatabaseIds.Add(datawalletModificationId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentDatabase()
    {
        var datawalletModificationId = DatawalletModificationId.New();

        _dataSource.DatabaseIds.Add(datawalletModificationId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().HaveCount(1).And.Contain(datawalletModificationId);
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentBlobs()
    {
        var datawalletModificationId = DatawalletModificationId.New();

        _dataSource.BlobIds.Add(datawalletModificationId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().HaveCount(1).And.Contain(datawalletModificationId);
    }
}
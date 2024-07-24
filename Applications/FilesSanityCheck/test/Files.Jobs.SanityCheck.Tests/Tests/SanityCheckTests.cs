using Backbone.FilesSanityCheck.Tests.Infrastructure.DataSource;
using Backbone.FilesSanityCheck.Tests.Infrastructure.Reporter;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;

namespace Backbone.FilesSanityCheck.Tests.Tests;

public class SanityCheckTests : AbstractTestsBase
{
    private readonly FakeDataSource _dataSource;
    private readonly TestReporter _reporter;
    private readonly FilesSanityCheck.Infrastructure.SanityCheck.SanityCheck _sanityCheck;

    public SanityCheckTests()
    {
        _dataSource = new FakeDataSource();
        _reporter = new TestReporter();
        _sanityCheck = new FilesSanityCheck.Infrastructure.SanityCheck.SanityCheck(_dataSource, _reporter);
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
        var fileId = FileId.New();

        _dataSource.BlobIds.Add(fileId);
        _dataSource.DatabaseIds.Add(fileId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentDatabase()
    {
        var fileId = FileId.New();

        _dataSource.DatabaseIds.Add(fileId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().HaveCount(1).And.Contain(fileId);
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentBlobs()
    {
        var fileId = FileId.New();

        _dataSource.BlobIds.Add(fileId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().HaveCount(1).And.Contain(fileId);
    }
}

using Backbone.Modules.Messages.Domain.Ids;
using FluentAssertions;
using Messages.Jobs.SanityCheck.Tests.Infrastructure.DataSource;
using Messages.Jobs.SanityCheck.Tests.Infrastructure.Reporter;
using Xunit;

namespace Messages.Jobs.SanityCheck.Tests.Tests;

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
        var messageId = MessageId.New();

        _dataSource.BlobIds.Add(messageId);
        _dataSource.DatabaseIds.Add(messageId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentDatabase()
    {
        var messageId = MessageId.New();

        _dataSource.DatabaseIds.Add(messageId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().HaveCount(1).And.Contain(messageId);
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentBlobs()
    {
        var messageId = MessageId.New();

        _dataSource.BlobIds.Add(messageId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().HaveCount(1).And.Contain(messageId);
    }
}

using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Jobs.SanityCheck.Tests.Infrastructure.DataSource;
using Backbone.Modules.Tokens.Jobs.SanityCheck.Tests.Infrastructure.Reporter;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Tokens.Jobs.SanityCheck.Tests.Tests;

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
        var tokenId = TokenId.New();

        _dataSource.BlobIds.Add(tokenId);
        _dataSource.DatabaseIds.Add(tokenId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentDatabase()
    {
        var tokenId = TokenId.New();

        _dataSource.DatabaseIds.Add(tokenId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().HaveCount(1).And.Contain(tokenId);
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentBlobs()
    {
        var tokenId = TokenId.New();

        _dataSource.BlobIds.Add(tokenId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().HaveCount(1).And.Contain(tokenId);
    }
}

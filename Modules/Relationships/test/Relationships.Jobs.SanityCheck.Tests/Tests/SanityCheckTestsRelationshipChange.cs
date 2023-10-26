using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.Modules.Relationships.Jobs.SanityCheck.Tests.Infrastructure.DataSource;
using Backbone.Modules.Relationships.Jobs.SanityCheck.Tests.Infrastructure.Reporter;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Jobs.SanityCheck.Tests.Tests;

public class SanityCheckTestsRelationshipChange
{
    private const string REQUEST_POSTFIX = "_Req";
    private const string RESPONSE_POSTFIX = "_Res";

    private readonly FakeDataSourceRelationshipChange _dataSource;
    private readonly TestReporterRelationshipChange _reporter;
    private readonly RelationshipChange.Infrastructure.SanityCheck.SanityCheck _sanityCheck;

    public SanityCheckTestsRelationshipChange()
    {
        _dataSource = new FakeDataSourceRelationshipChange();
        _reporter = new TestReporterRelationshipChange();
        _sanityCheck = new RelationshipChange.Infrastructure.SanityCheck.SanityCheck(_dataSource, _reporter);
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
        var relationshipChangeId = RelationshipChangeId.New();
        var relationshipChangeRequestId = relationshipChangeId + REQUEST_POSTFIX;
        var relationshipChangeResponseId = relationshipChangeId + RESPONSE_POSTFIX;

        _dataSource.BlobIds.Add(relationshipChangeRequestId);
        _dataSource.BlobIds.Add(relationshipChangeResponseId);
        _dataSource.DatabaseIds.Add(relationshipChangeId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckMissingResponseNotReported()
    {
        var relationshipChangeId = RelationshipChangeId.New();
        var relationshipChangeRequestId = relationshipChangeId + REQUEST_POSTFIX;

        _dataSource.BlobIds.Add(relationshipChangeRequestId);
        _dataSource.DatabaseIds.Add(relationshipChangeId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentDatabase()
    {
        var relationshipChangeId = RelationshipChangeId.New();
        var relationshipChangeResponseId = relationshipChangeId + RESPONSE_POSTFIX;

        _dataSource.BlobIds.Add(relationshipChangeResponseId);
        _dataSource.DatabaseIds.Add(relationshipChangeId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().HaveCount(1).And.Contain(relationshipChangeId);
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentBlobs()
    {
        var relationshipChangeId = RelationshipChangeId.New();
        var relationshipChangeRequestId = relationshipChangeId + REQUEST_POSTFIX;
        var relationshipChangeResponseId = relationshipChangeId + RESPONSE_POSTFIX;

        _dataSource.BlobIds.Add(relationshipChangeRequestId);
        _dataSource.BlobIds.Add(relationshipChangeResponseId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().HaveCount(2).And.Contain(relationshipChangeRequestId).And.Contain(relationshipChangeResponseId);
    }
}

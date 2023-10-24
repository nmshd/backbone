using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.Modules.Relationships.Jobs.SanityCheck.Tests.Infrastructure.DataSource;
using Backbone.Modules.Relationships.Jobs.SanityCheck.Tests.Infrastructure.Reporter;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Jobs.SanityCheck.Tests.Tests;

public class SanityCheckTestsRelationshipTemplate
{
    private readonly FakeDataSourceRelationshipTemplate _dataSource;
    private readonly TestReporterRelationshipTemplate _reporter;
    private readonly RelationshipTemplate.Infrastructure.SanityCheck.SanityCheck _sanityCheck;

    public SanityCheckTestsRelationshipTemplate()
    {
        _dataSource = new FakeDataSourceRelationshipTemplate();
        _reporter = new TestReporterRelationshipTemplate();
        _sanityCheck = new RelationshipTemplate.Infrastructure.SanityCheck.SanityCheck(_dataSource, _reporter);
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
        var relationshipTemplateId = RelationshipTemplateId.New();

        _dataSource.BlobIds.Add(relationshipTemplateId);
        _dataSource.DatabaseIds.Add(relationshipTemplateId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentDatabase()
    {
        var relationshipTemplateId = RelationshipTemplateId.New();

        _dataSource.DatabaseIds.Add(relationshipTemplateId);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().HaveCount(1).And.Contain(relationshipTemplateId);
        _reporter.ReportedBlobIds.Should().BeEmpty();
    }

    [Fact]
    public async Task SanityCheckInconsistentBlobs()
    {
        var relationshipTemplate = RelationshipTemplateId.New();

        _dataSource.BlobIds.Add(relationshipTemplate);

        await _sanityCheck.Run(CancellationToken.None);

        _reporter.ReportedDatabaseIds.Should().BeEmpty();
        _reporter.ReportedBlobIds.Should().HaveCount(1).And.Contain(relationshipTemplate);

    }
}

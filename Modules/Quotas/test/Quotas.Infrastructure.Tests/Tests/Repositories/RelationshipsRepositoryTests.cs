using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Tooling;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Quotas.Infrastructure.Tests.Tests.Repositories;
public class RelationshipsRepositoryTests
{
    private readonly RelationshipsDbContext _relationshipsArrangeContext;
    private readonly QuotasDbContext _actContext;

    public RelationshipsRepositoryTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        var connection = FakeDbContextFactory.CreateDbConnection();
        (_relationshipsArrangeContext, _, _) = FakeDbContextFactory.CreateDbContexts<RelationshipsDbContext>(connection);
        (_, _, _actContext) = FakeDbContextFactory.CreateDbContexts<QuotasDbContext>(connection);

        SystemTime.Set(DateTime.UtcNow);
    }

    [Fact]
    public async Task Count_pending_relationships()
    {
        // Arrange
        var relationships = new List<Relationship>()
        {
            CreatePendingRelationship(),
            CreatePendingRelationship()
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var count = await repository.Count(IDENTITY_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public async Task Count_active_relationships()
    {
        // Arrange
        var relationships = new List<Relationship>()
        {
            CreateActiveRelationship(),
            CreateActiveRelationship(),
            CreateActiveRelationship()
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var count = await repository.Count(IDENTITY_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task Does_not_count_terminated_relationships()
    {
        // Arrange
        var relationships = new List<Relationship>()
        {
            CreateTerminatedRelationship()

        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var count = await repository.Count(IDENTITY_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public async Task Count_relationships_where_reactivation_is_requested()
    {
        // Arrange
        var relationships = new List<Relationship>()
        {
            CreateRelationshipWithRequestedReactivation()
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var count = await repository.Count(IDENTITY_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(1);
    }
}

using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
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
    }

    [Fact]
    public async Task Counts_pending_relationships()
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
        var firstParticipantCount = await repository.Count(IDENTITY_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);
        var secondParticipantCount = await repository.Count(IDENTITY_2, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        firstParticipantCount.Should().Be(2);
        secondParticipantCount.Should().Be(0);
    }

    [Fact]
    public async Task Counts_active_relationships()
    {
        // Arrange
        var relationships = new List<Relationship>()
        {
            CreateActiveRelationship(),
            CreateActiveRelationship()
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var firstParticipantCount = await repository.Count(IDENTITY_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);
        var secondParticipantCount = await repository.Count(IDENTITY_2, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        firstParticipantCount.Should().Be(2);
        secondParticipantCount.Should().Be(2);
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
        var firstParticipantCount = await repository.Count(IDENTITY_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);
        var secondParticipantCount = await repository.Count(IDENTITY_2, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        firstParticipantCount.Should().Be(0);
        secondParticipantCount.Should().Be(0);
    }

    [Fact]
    public async Task Counts_relationships_where_reactivation_is_requested_by_first_participant()
    {
        // Arrange
        var relationships = new List<Relationship>()
        {
            CreateRelationshipWithRequestedReactivation(IDENTITY_1)
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var firstParticipantCount = await repository.Count(IDENTITY_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);
        var secondParticipantCount = await repository.Count(IDENTITY_2, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        firstParticipantCount.Should().Be(1);
        secondParticipantCount.Should().Be(0);
    }

    [Fact]
    public async Task Counts_relationships_where_reactivation_is_requested_by_second_participant()
    {
        // Arrange
        var relationships = new List<Relationship>()
        {
            CreateRelationshipWithRequestedReactivation(IDENTITY_2)
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var firstParticipantCount = await repository.Count(IDENTITY_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);
        var secondParticipantCount = await repository.Count(IDENTITY_2, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        firstParticipantCount.Should().Be(0);
        secondParticipantCount.Should().Be(1);
    }
}

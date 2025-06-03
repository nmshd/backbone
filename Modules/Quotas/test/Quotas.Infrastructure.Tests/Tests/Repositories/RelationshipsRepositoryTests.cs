using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Tooling;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using static Backbone.Modules.Quotas.Infrastructure.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Quotas.Infrastructure.Tests.Tests.Repositories;

public class RelationshipsRepositoryTests : AbstractTestsBase
{
    private static readonly IdentityAddress I1 = IdentityAddress.Create([2, 2, 2], "enmeshed.eu");
    private static readonly IdentityAddress I2 = IdentityAddress.Create([1, 1, 1], "enmeshed.eu");
    private static readonly IdentityAddress I3 = IdentityAddress.Create([1, 0, 1], "enmeshed.eu");
    private static readonly IdentityAddress I4 = IdentityAddress.Create([1, 4, 1], "enmeshed.eu");

    private readonly RelationshipsDbContext _relationshipsArrangeContext;
    private readonly QuotasDbContext _actContext;

    public RelationshipsRepositoryTests()
    {
        var connection = FakeDbContextFactory.CreateDbConnection();
        (_relationshipsArrangeContext, _, _) = FakeDbContextFactory.CreateDbContexts<RelationshipsDbContext>(connection);
        (_, _, _actContext) = FakeDbContextFactory.CreateDbContexts<QuotasDbContext>(connection);
    }

    [Fact]
    public async Task Pending_relationships_count_for_from_only()
    {
        // Arrange
        var relationships = new List<Relationship>
        {
            CreatePendingRelationship(I1, I2),
            CreatePendingRelationship(I2, I1)
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var countForI1 = await repository.Count(I1, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);
        var countForI2 = await repository.Count(I2, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);

        // Assert
        countForI1.ShouldBe(1u);
        countForI2.ShouldBe(1u);
    }

    [Fact]
    public async Task Active_relationships_count_for_both_participants()
    {
        // Arrange
        var relationships = new List<Relationship>
        {
            CreateActiveRelationship(I1, I2),
            CreateActiveRelationship(I2, I1)
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var countForI1 = await repository.Count(I1, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);
        var countForI2 = await repository.Count(I2, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);

        // Assert
        countForI1.ShouldBe(2u);
        countForI2.ShouldBe(2u);
    }

    [Fact]
    public async Task Terminated_relationships_count_for_both_participants()
    {
        // Arrange
        var relationships = new List<Relationship>
        {
            CreateTerminatedRelationship(I1, I2),
            CreateTerminatedRelationship(I2, I1)
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var countForI1 = await repository.Count(I1, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);
        var countForI2 = await repository.Count(I2, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);

        // Assert
        countForI1.ShouldBe(2u);
        countForI2.ShouldBe(2u);
    }

    [Fact]
    public async Task Requested_reactivation_outside_given_quota_period_does_not_count_for_any_participant()
    {
        // Arrange
        var relationships = new List<Relationship>
        {
            CreateRelationshipWithRequestedReactivation(from: I1, to: I2, reactivationRequestedBy: I1),
            CreateRelationshipWithRequestedReactivation(from: I2, to: I1, reactivationRequestedBy: I2)
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var countForI1 = await repository.Count(I1, quotaPeriod.CalculateBegin(SystemTime.UtcNow).AddHours(2), quotaPeriod.CalculateEnd(SystemTime.UtcNow).AddHours(2), CancellationToken.None);
        var countForI2 = await repository.Count(I2, quotaPeriod.CalculateBegin(SystemTime.UtcNow).AddHours(2), quotaPeriod.CalculateEnd(SystemTime.UtcNow).AddHours(2), CancellationToken.None);

        // Assert
        countForI1.ShouldBe(0u);
        countForI2.ShouldBe(0u);
    }

    [Fact]
    public async Task Other_participants_do_not_count()
    {
        // Arrange
        var relationships = new List<Relationship>
        {
            CreateActiveRelationship(I3, I4),
            CreateActiveRelationship(I2, I4),
            CreateActiveRelationship(I2, I3)
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var count = await repository.Count(I1, quotaPeriod.CalculateBegin(SystemTime.UtcNow).AddHours(2), quotaPeriod.CalculateEnd(SystemTime.UtcNow).AddHours(2), CancellationToken.None);

        // Assert
        count.ShouldBe(0u);
    }

    [Fact]
    public async Task Decomposed_relationships_count_for_peer_only()
    {
        // Arrange
        var relationships = new List<Relationship>
        {
            CreateDecomposedRelationship(from: I1, to: I2, decomposedBy: I1),
            CreateDecomposedRelationship(from: I1, to: I2, decomposedBy: I2)
        };
        await _relationshipsArrangeContext.Relationships.AddRangeAsync(relationships);
        await _relationshipsArrangeContext.SaveChangesAsync();

        var repository = new RelationshipsRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var countForI1 = await repository.Count(I1, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);
        var countForI2 = await repository.Count(I2, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);

        // Assert
        countForI1.ShouldBe(1u);
        countForI2.ShouldBe(1u);
    }
}

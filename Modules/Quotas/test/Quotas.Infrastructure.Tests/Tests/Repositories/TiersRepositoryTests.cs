using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Backbone.UnitTestTools.TestDoubles.Fakes;

namespace Backbone.Modules.Quotas.Infrastructure.Tests.Tests.Repositories;

public class TiersRepositoryTests : AbstractTestsBase
{
    /**
     * This test makes sure that when the Update method is called on the TiersRepository with a Tier from which
     * TierQuotaDefinitions were removed, the TierQuotaDefinitions were removed from the database as well.
     * That's not the case by default, because EF Core only sets the foreign key to null, but doesn't remove the
     * lines from the TierQuotaDefinitions table.
     */
    [Fact]
    public async Task Updating_a_Tier_deletes_unassigned_quotas_from_the_TierQuotaDefinitions_table()
    {
        // Arrange
        var (arrangeContext, actContext, assertContext) = FakeDbContextFactory.CreateDbContexts<QuotasDbContext>();

        var arrangedTier = new Tier(TierId.Parse("TIR00000000000000000"), "Test");
        var tierQuotaDefinitionToBeDeleted = arrangedTier.CreateQuota(MetricKey.NUMBER_OF_SENT_MESSAGES, 5, QuotaPeriod.Month).Value;
        var otherTierQuotaDefinition = arrangedTier.CreateQuota(MetricKey.NUMBER_OF_FILES, 5, QuotaPeriod.Month).Value;

        await arrangeContext.Tiers.AddAsync(arrangedTier);
        await arrangeContext.SaveChangesAsync();

        var repository = new TiersRepository(actContext);

        var actTier = (await repository.Find(arrangedTier.Id, CancellationToken.None, true))!;
        actTier.DeleteQuota(tierQuotaDefinitionToBeDeleted.Id);

        // Act
        await repository.Update(actTier, CancellationToken.None);

        // Assert
        assertContext.Set<TierQuotaDefinition>().ShouldNotContain(q => q.Id == tierQuotaDefinitionToBeDeleted.Id);
        assertContext.Set<TierQuotaDefinition>().ShouldContain(q => q.Id == otherTierQuotaDefinition.Id);
    }
}

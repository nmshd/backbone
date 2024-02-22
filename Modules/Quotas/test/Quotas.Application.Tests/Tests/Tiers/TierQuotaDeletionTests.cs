using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Tiers;

public class TierQuotaDeletionTests
{
    private readonly QuotasDbContext _actContext;
    private readonly QuotasDbContext _arrangeContext;
    private readonly QuotasDbContext _assertContext;

    private TierId? _tierId;
    private TierQuotaDefinitionId? _tierQuotaDefinitionId;

    public TierQuotaDeletionTests()
    {
        (_arrangeContext, _actContext, _assertContext) = FakeDbContextFactory.CreateDbContexts<QuotasDbContext>();
    }

    [Fact]
    public async void Tier_quota_definition_is_deleted_when_removed_from_its_tier()
    {
        // Arrange
        await CreateTier(ContextToUse.Act);

        // Act
        await DeleteTierQuotaDefinition(ContextToUse.Act, _tierId!, _tierQuotaDefinitionId!);

        // Assert
        var repository = CreateRepository(ContextToUse.Assert);
        var asserting =
            async () => await repository.FindTierQuotaDefinition(_tierQuotaDefinitionId!.Value, CancellationToken.None);

        await asserting.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async void Tier_quota_definition_is_deleted_when_its_tier_is_deleted()
    {
        // Arrange
        await CreateTier(ContextToUse.Act);

        // Act
        await DeleteTier(ContextToUse.Act, _tierId!);

        // Assert
        var repository = CreateRepository(ContextToUse.Assert);
        var asserting =
            async () => await repository.FindTierQuotaDefinition(_tierQuotaDefinitionId!.Value, CancellationToken.None);

        await asserting.Should().ThrowAsync<NotFoundException>();
    }

    private async Task CreateTier(ContextToUse contextToUse)
    {
        var repository = CreateRepository(contextToUse);

        _tierId = new TierId("TIR00000000000000000");
        var tier = new Tier(_tierId, "Test");

        var metricKey = MetricKey.NumberOfSentMessages;
        const int max = 5;
        const QuotaPeriod period = QuotaPeriod.Month;

        tier.CreateQuota(metricKey, max, period);
        _tierQuotaDefinitionId = tier.Quotas.First().Id;

        await repository.Add(tier, CancellationToken.None);
        await _arrangeContext.SaveChangesAsync();
    }

    private async Task DeleteTierQuotaDefinition(ContextToUse contextToUse, TierId tierId, TierQuotaDefinitionId tierQuotaDefinitionId)
    {
        var repository = CreateRepository(contextToUse);
        var context = GetContext(contextToUse);

        var tier = await repository.Find(tierId, CancellationToken.None, true) ?? throw new NotFoundException(nameof(Tier));

        tier.DeleteQuota(tierQuotaDefinitionId);

        await repository.Update(tier, CancellationToken.None);
        await context.SaveChangesAsync();
    }

    private async Task DeleteTier(ContextToUse contextToUse, TierId tierId)
    {
        var repository = CreateRepository(contextToUse);
        var context = GetContext(contextToUse);

        await repository.RemoveById(tierId);
        await context.SaveChangesAsync();
    }

    private QuotasDbContext GetContext(ContextToUse contextToUse)
    {
        switch (contextToUse)
        {
            case ContextToUse.Arrange:
                return _arrangeContext;
            case ContextToUse.Act:
                return _actContext;
            case ContextToUse.Assert:
                return _assertContext;
            default:
                throw new ArgumentOutOfRangeException(nameof(contextToUse), contextToUse, null);
        }
    }

    private TiersRepository CreateRepository(ContextToUse contextToUse)
    {
        switch (contextToUse)
        {
            case ContextToUse.Arrange:
                return new TiersRepository(_arrangeContext);
            case ContextToUse.Act:
                return new TiersRepository(_actContext);
            case ContextToUse.Assert:
                return new TiersRepository(_assertContext);
            default:
                throw new ArgumentOutOfRangeException(nameof(contextToUse), contextToUse, null);
        }
    }

    private enum ContextToUse
    {
        Arrange,
        Act,
        Assert
    }
}

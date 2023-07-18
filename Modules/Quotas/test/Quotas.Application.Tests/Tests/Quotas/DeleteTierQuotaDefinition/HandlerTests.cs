using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using FakeItEasy;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.DeleteTierQuotaDefinition;
public class HandlerTests
{
    private readonly IEventBus _eventBus;

    public HandlerTests()
    {
        _eventBus = A.Fake<IEventBus>();
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;
    }

    [Fact]
    public async Task Deletes_quota_for_tier()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var tier = new Tier(tierId, "some-tier-name");

        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);

        var command = new DeleteTierQuotaDefinitionCommand(tier.Id, tier.Quotas.First().Id);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(tier);

        var handler = CreateHandler(tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => tiersRepository.Update(A<Tier>.That.Matches(t =>
                t.Id == tierId &&
                t.Quotas.Count == 0)
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Deletes_quota_for_tier_with_multiple_quotas()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var tier = new Tier(tierId, "some-tier-name");

        var metricKey = MetricKey.NumberOfSentMessages;
        var max = 5;
        var period = QuotaPeriod.Month;
        tier.CreateQuota(metricKey, max, period);
        tier.CreateQuota(metricKey, max, period);
        tier.CreateQuota(metricKey, max, period);

        var command = new DeleteTierQuotaDefinitionCommand(tier.Id, tier.Quotas.First().Id);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(tier);

        var handler = CreateHandler(tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => tiersRepository.Update(A<Tier>.That.Matches(t =>
                t.Id == tierId &&
                t.Quotas.Count == 2)
            , CancellationToken.None)
        ).MustHaveHappened();
    }

    [Fact]
    public async Task Triggers_TierQuotaDefinitionDeletedIntegrationEvent()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var tier = new Tier(tierId, "some-tier-name");

        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);

        var command = new DeleteTierQuotaDefinitionCommand(tier.Id, tier.Quotas.First().Id);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(tier);

        var handler = CreateHandler(tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _eventBus.Publish(A<IntegrationEvent>.That.IsInstanceOf(typeof(TierQuotaDefinitionDeletedIntegrationEvent)))).MustHaveHappened();
    }

    private Handler CreateHandler(ITiersRepository tiersRepository)
    {
        var logger = A.Fake<ILogger<Handler>>();

        return new Handler(tiersRepository, logger, _eventBus);
    }
}

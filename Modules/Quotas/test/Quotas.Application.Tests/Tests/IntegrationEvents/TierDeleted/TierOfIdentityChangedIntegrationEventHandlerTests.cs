using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using Enmeshed.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.IntegrationEvents.TierDeleted;
public class TierOfIdentityChangedIntegrationEventHandlerTests
{
    [Fact]
    public async Task Updates_identity_with_new_tier()
    {
        // Arrange
        var tiersRepository = A.Fake<ITiersRepository>();

        var oldTier = new Tier(new("old-tier-id"), "old-tier");
        var newTier = new Tier(new("new-tier-id"), "new-tier");

        A.CallTo(() => tiersRepository.Find(oldTier.Id, A<CancellationToken>._, A<bool>._)).Returns(oldTier);
        A.CallTo(() => tiersRepository.Find(newTier.Id, A<CancellationToken>._, A<bool>._)).Returns(newTier);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();

        var identity = new Identity(TestDataGenerator.CreateRandomIdentityAddress(), oldTier.Id);

        A.CallTo(() => identitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var metricCalculatorFactory = A.Fake<MetricCalculatorFactory>();

        var handler = CreateHandler(identitiesRepository, tiersRepository, metricCalculatorFactory);

        var tierDeletedIntegrationEvent = new TierOfIdentityChangedIntegrationEvent()
        {
            OldTier = oldTier.Id,
            NewTier = newTier.Id,
            IdentityAddress = identity.Address
        };

        // Act
        await handler.Handle(tierDeletedIntegrationEvent);

        // Assert
        A.CallTo(() => identitiesRepository.Update(identity, A<CancellationToken>._)).MustHaveHappened();
        identity.TierId.Should().Be(newTier.Id);
    }

    [Fact]
    public async Task Handler_creates_MetricStatuses()
    {
        // Arrange
        var tiersRepository = A.Fake<ITiersRepository>();

        var oldTier = new Tier(new("old-tier-id"), "old-tier");
        var newTier = new Tier(new("new-tier-id"), "new-tier");
        newTier.Quotas.Add(new(MetricKey.NumberOfFiles, 5, QuotaPeriod.Day));

        A.CallTo(() => tiersRepository.Find(oldTier.Id, A<CancellationToken>._, A<bool>._)).Returns(oldTier);
        A.CallTo(() => tiersRepository.Find(newTier.Id, A<CancellationToken>._, A<bool>._)).Returns(newTier);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();

        var identity = new Identity(TestDataGenerator.CreateRandomIdentityAddress(), oldTier.Id);

        A.CallTo(() => identitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var metricCalculatorFactory = A.Fake<ServiceProviderMetricCalculatorFactory>();

        var handler = CreateHandler(identitiesRepository, tiersRepository, metricCalculatorFactory);

        var tierDeletedIntegrationEvent = new TierOfIdentityChangedIntegrationEvent()
        {
            OldTier = oldTier.Id,
            NewTier = newTier.Id,
            IdentityAddress = identity.Address
        };

        // Act
        await handler.Handle(tierDeletedIntegrationEvent);

        // Assert
        identity.MetricStatuses.Should().HaveCount(1);
        identity.MetricStatuses.First().MetricKey.Should().Be(newTier.Quotas.First().MetricKey);
    }


    [Fact]
    public async Task Handler_clears_old_MetricStatuses()
    {
        // Arrange
        var tiersRepository = A.Fake<ITiersRepository>();

        var oldTier = new Tier(new("old-tier-id"), "old-tier");
        oldTier.Quotas.Add(new(MetricKey.NumberOfTokens, 5, QuotaPeriod.Day));

        var newTier = new Tier(new("new-tier-id"), "new-tier");
        newTier.Quotas.Add(new(MetricKey.NumberOfFiles, 5, QuotaPeriod.Day));

        A.CallTo(() => tiersRepository.Find(oldTier.Id, A<CancellationToken>._, A<bool>._)).Returns(oldTier);
        A.CallTo(() => tiersRepository.Find(newTier.Id, A<CancellationToken>._, A<bool>._)).Returns(newTier);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();

        var identity = new Identity(TestDataGenerator.CreateRandomIdentityAddress(), oldTier.Id);

        A.CallTo(() => identitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var metricCalculatorFactory = A.Fake<ServiceProviderMetricCalculatorFactory>();

        identity.AssignTierQuotaFromDefinition(oldTier.Quotas.First());
        await identity.UpdateMetricStatuses(oldTier.Quotas.Select(q => q.MetricKey), metricCalculatorFactory, CancellationToken.None);

        identity.MetricStatuses.Should().HaveCount(1);
        identity.MetricStatuses.First().MetricKey.Should().Be(oldTier.Quotas.First().MetricKey);

        var handler = CreateHandler(identitiesRepository, tiersRepository, metricCalculatorFactory);

        var tierDeletedIntegrationEvent = new TierOfIdentityChangedIntegrationEvent()
        {
            OldTier = oldTier.Id,
            NewTier = newTier.Id,
            IdentityAddress = identity.Address
        };

        // Act
        await handler.Handle(tierDeletedIntegrationEvent);

        // Assert
        identity.MetricStatuses.Should().HaveCount(1);
        identity.MetricStatuses.First().MetricKey.Should().Be(newTier.Quotas.First().MetricKey);
    }

    private TierOfIdentityChangedIntegrationEventHandler CreateHandler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, MetricCalculatorFactory metricCalculatorFactory)
    {
        return new(identitiesRepository, tiersRepository, metricCalculatorFactory);
    }
}

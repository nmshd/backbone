using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierDeleted;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.IntegrationEvents.TierDeleted;
public class TierOfIdentityChangedIntegrationEventHandlerTests
{
    private readonly ILogger<TierOfIdentityChangedIntegrationEventHandler> _logger;

    public TierOfIdentityChangedIntegrationEventHandlerTests()
    {
        _logger = A.Fake<ILogger<TierOfIdentityChangedIntegrationEventHandler>>();
    }

    [Fact]
    public async Task Updates_identity_with_new_tier_and_calls_MetricStatusService()
    {
        // Arrange
        var tiersRepository = A.Fake<ITiersRepository>();

        var oldTier = new Tier(new("old-tier-id"), "old-tier");
        var newTier = new Tier(new("new-tier-id"), "new-tier");

        A.CallTo(() => tiersRepository.Find(oldTier.Id, A<CancellationToken>._, A<bool>._)).Returns(oldTier);
        A.CallTo(() => tiersRepository.Find(newTier.Id, A<CancellationToken>._, A<bool>._)).Returns(newTier);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();

        var identity = new Identity(TestDataGenerator.CreateRandomIdentityAddress(), oldTier.Id);

        A.CallTo(()=>identitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var metricStatusesService = A.Fake<IMetricStatusesService>();

        var handler = CreateHandler(identitiesRepository, tiersRepository, metricStatusesService);

        var tierDeletedIntegrationEvent = new TierOfIdentityChangedIntegrationEvent()
        {
            OldTier = oldTier.Id,
            NewTier = newTier.Id,
            IdentityAddress = identity.Address

        };

        // Act
        await handler.Handle(tierDeletedIntegrationEvent);

        // Assert
        A.CallTo(metricStatusesService).Where(x=>x.Method.Name== nameof(metricStatusesService.RecalculateMetricStatuses)).WithAnyArguments().MustHaveHappened();
        A.CallTo(() => identitiesRepository.Update(identity, A<CancellationToken>._)).MustHaveHappened();
        identity.TierId.Should().Be(newTier.Id);
    }

    private TierOfIdentityChangedIntegrationEventHandler CreateHandler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, IMetricStatusesService metricStatusesService)
    {
        return new(identitiesRepository, tiersRepository, metricStatusesService);
    }
}

using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierDeleted;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.IntegrationEvents.TierDeleted;
public class TierDeletedIntegrationEventHandlerTests
{
    private readonly ILogger<TierDeletedIntegrationEventHandler> _logger;

    public TierDeletedIntegrationEventHandlerTests()
    {
        _logger = A.Fake<ILogger<TierDeletedIntegrationEventHandler>>();
    }

    [Fact]
    public async Task Deletes_tier_after_consuming_integration_event()
    {
        // Arrange
        var tiersRepository = A.Fake<ITiersRepository>();
        var tier = new Tier(new("tier-id"), "tier-name");
        A.CallTo(() => tiersRepository.Find(tier.Id, A<CancellationToken>._, A<bool>._)).Returns(tier);
        var handler = CreateHandler(tiersRepository);

        var tierDeletedIntegrationEvent = new TierDeletedIntegrationEvent(tier.Id);

        // Act
        await handler.Handle(tierDeletedIntegrationEvent);

        // Assert
        A.CallTo(() => tiersRepository.Remove(tier)).MustHaveHappenedOnceExactly();
    }

    private TierDeletedIntegrationEventHandler CreateHandler(ITiersRepository tiersRepository)
    {
        return new(_logger, tiersRepository);
    }
}

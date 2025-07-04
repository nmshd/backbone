using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierDeleted;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierDeleted;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.DomainEvents.TierDeleted;

public class TierDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Deletes_tier_after_consuming_domain_event()
    {
        // Arrange
        var tiersRepository = A.Fake<ITiersRepository>();
        var tier = new Tier(TierId.Parse("tier-id"), "tier-name");
        A.CallTo(() => tiersRepository.Get(tier.Id, A<CancellationToken>._, A<bool>._)).Returns(tier);
        var handler = CreateHandler(tiersRepository);

        var tierDeletedDomainEvent = new TierDeletedDomainEvent(tier.Id);

        // Act
        await handler.Handle(tierDeletedDomainEvent);

        // Assert
        A.CallTo(() => tiersRepository.RemoveById(tier.Id)).MustHaveHappenedOnceExactly();
    }

    private static TierDeletedDomainEventHandler CreateHandler(ITiersRepository tiersRepository)
    {
        return new TierDeletedDomainEventHandler(A.Dummy<ILogger<TierDeletedDomainEventHandler>>(), tiersRepository);
    }
}

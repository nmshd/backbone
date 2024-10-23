using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Domain.Metrics;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;

public class IdentityCreatedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_a_copy_of_the_identity_from_the_event()
    {
        // Arrange
        const string address = "some-dummy-address";
        var tierId = TierId.Parse("TIRsomeTierId1111111");
        var tier = new Tier(tierId, "some-tier-name");
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var stubTiersRepository = new FindTiersStubRepository(tier);
        var handler = CreateHandler(mockIdentitiesRepository, stubTiersRepository);

        // Act
        await handler.Handle(new IdentityCreatedDomainEvent(address, tierId));

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Add(A<Identity>.That.Matches(i => i.Address == address && i.TierId == tierId), CancellationToken.None)).MustHaveHappened();
    }

    [Fact]
    public async Task Assigns_tier_quotas_to_new_identity()
    {
        // Arrange
        const string address = "some-dummy-address";
        var tierId = TierId.Parse("TIRsomeTierId1111111");

        const int max = 5;
        var tier = new Tier(tierId, "some-tier-name");
        tier.Quotas.Add(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, max, QuotaPeriod.Month));
        tier.Quotas.Add(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, max, QuotaPeriod.Week));

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var stubTiersRepository = new FindTiersStubRepository(tier);

        var handler = CreateHandler(mockIdentitiesRepository, stubTiersRepository);

        // Act
        await handler.Handle(new IdentityCreatedDomainEvent(address, tierId));

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Add(A<Identity>.That.Matches(i => i.TierQuotas.Count == 2), CancellationToken.None)).MustHaveHappened();
    }

    private static IdentityCreatedDomainEventHandler CreateHandler(IIdentitiesRepository identities, FindTiersStubRepository tiers)
    {
        var logger = A.Fake<ILogger<IdentityCreatedDomainEventHandler>>();
        var metricCalculatorFactory = A.Fake<MetricCalculatorFactory>();
        return new IdentityCreatedDomainEventHandler(identities, logger, tiers, metricCalculatorFactory);
    }
}

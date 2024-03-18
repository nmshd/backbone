using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;

public class IdentityCreatedIntegrationEventHandlerTests
{
    [Fact]
    public async void Creates_a_copy_of_the_identity_from_the_event()
    {
        // Arrange
        var address = "some-dummy-address";
        var tierId = new TierId("TIRsomeTierId1111111");
        var tier = new Tier(tierId, "some-tier-name");
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var stubTiersRepository = new FindTiersStubRepository(tier);
        var handler = CreateHandler(mockIdentitiesRepository, stubTiersRepository);

        // Act
        await handler.Handle(new IdentityCreatedIntegrationEvent(address, tierId));

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Add(A<Identity>.That.Matches(i => i.Address == address && i.TierId == tierId), CancellationToken.None)).MustHaveHappened();
    }

    [Fact]
    public async void Assigns_tier_quotas_to_new_identity()
    {
        // Arrange
        var address = "some-dummy-address";
        var tierId = new TierId("TIRsomeTierId1111111");

        var max = 5;
        var tier = new Tier(tierId, "some-tier-name");
        tier.Quotas.Add(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, max, QuotaPeriod.Month));
        tier.Quotas.Add(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, max, QuotaPeriod.Week));

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var stubTiersRepository = new FindTiersStubRepository(tier);

        var handler = CreateHandler(mockIdentitiesRepository, stubTiersRepository);

        // Act
        await handler.Handle(new IdentityCreatedIntegrationEvent(address, tierId));

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Add(A<Identity>.That.Matches(i => i.TierQuotas.Count == 2), CancellationToken.None)).MustHaveHappened();
    }

    private IdentityCreatedIntegrationEventHandler CreateHandler(IIdentitiesRepository identities, FindTiersStubRepository tiers)
    {
        var logger = A.Fake<ILogger<IdentityCreatedIntegrationEventHandler>>();
        var metricCalculatorFactory = A.Fake<MetricCalculatorFactory>();
        return new IdentityCreatedIntegrationEventHandler(identities, logger, tiers, metricCalculatorFactory);
    }
}

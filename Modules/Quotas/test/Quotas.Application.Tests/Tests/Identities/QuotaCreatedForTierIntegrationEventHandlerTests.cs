using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.QuotaCreatedForTier;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;

public class QuotaCreatedForTierIntegrationEventHandlerTests
{
    [Fact]
    public async void Creates_tier_quota_after_consuming_integration_event()
    {
        // Arrange
        var tierId = new TierId("TIRFxoL0U24aUqZDSAWc");
        var tier = new Tier(tierId, "some-tier-name");

        var max = 5;
        var period = QuotaPeriod.Month;
        var tierQuotaDefinition = new TierQuotaDefinition(MetricKey.NumberOfSentMessages, max, period);
        var tierQuotaDefinitionsRepository = new FindTierQuotaDefinitionsStubRepository(tierQuotaDefinition);

        var firstIdentity = new Identity("some-identity-address-one", tierId);
        var secondIdentity = new Identity("some-identity-address-two", tierId);
        var identities = new List<Identity> { firstIdentity, secondIdentity };
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.FindWithTier(tierId, CancellationToken.None, true)).Returns(identities);

        var handler = CreateHandler(identitiesRepository, tierQuotaDefinitionsRepository);

        // Act
        await handler.Handle(new QuotaCreatedForTierIntegrationEvent(tier, tierQuotaDefinition));

        // Assert
        A.CallTo(() => identitiesRepository.Update(A<IEnumerable<Identity>>.That.Matches(identities =>
            identities.All(i => i.TierQuotas.Count == 1))
            , CancellationToken.None)
        ).MustHaveHappened();
    }

    private QuotaCreatedForTierIntegrationEventHandler CreateHandler(IIdentitiesRepository identities, FindTierQuotaDefinitionsStubRepository tierQuotaDefinitions)
    {
        var logger = A.Fake<ILogger<QuotaCreatedForTierIntegrationEventHandler>>();
        return new QuotaCreatedForTierIntegrationEventHandler(identities, tierQuotaDefinitions, logger);
    }
}

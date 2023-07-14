using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.QuotaDeletedForTier;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;
public class QuotaDeletedForTierIntegrationEventHandlerTests
{
    [Fact]
    public async void Deletes_tier_quota_after_consuming_integration_event()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var tier = new Tier(tierId, "some-tier-name");

        var tierQuotaDefinition = new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        var tierQuotaDefinitionsRepository = new FindTierQuotaDefinitionsStubRepository(tierQuotaDefinition);

        var firstIdentity = new Identity("some-identity-address-one", tierId);
        firstIdentity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        var secondIdentity = new Identity("some-identity-address-two", tierId);
        secondIdentity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        var identities = new List<Identity> { firstIdentity, secondIdentity };
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.FindWithTier(tierId, CancellationToken.None, true)).Returns(identities);

        var handler = CreateHandler(identitiesRepository, tierQuotaDefinitionsRepository);

        // Act
        await handler.Handle(new QuotaDeletedForTierIntegrationEvent(tier.Id, tierQuotaDefinition.Id));

        // Assert
        A.CallTo(() => identitiesRepository.Update(A<IEnumerable<Identity>>.That.Matches(ids =>
                ids.All(i => i.TierQuotas.Count == 0))
            , CancellationToken.None)
        ).MustHaveHappened();
    }

    private QuotaDeletedForTierIntegrationEventHandler CreateHandler(IIdentitiesRepository identities, FindTierQuotaDefinitionsStubRepository tierQuotaDefinitions)
    {
        var logger = A.Fake<ILogger<QuotaDeletedForTierIntegrationEventHandler>>();
        return new QuotaDeletedForTierIntegrationEventHandler(identities, tierQuotaDefinitions, logger);
    }
}

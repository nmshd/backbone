using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierQuotaDefinitionCreated;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Outgoing;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;

public class TierQuotaDefinitionCreatedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_tier_quota_after_consuming_domain_event()
    {
        // Arrange
        var tierId = TierId.Parse("TIRFxoL0U24aUqZDSAWc");

        var tierQuotaDefinition = new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 5, QuotaPeriod.Month);
        var tierQuotaDefinitionsRepository = new FindTierQuotaDefinitionsStubRepository(tierQuotaDefinition);

        var firstIdentity = new Identity("some-identity-address-one", tierId);
        var secondIdentity = new Identity("some-identity-address-two", tierId);
        var identities = new List<Identity> { firstIdentity, secondIdentity };
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.ListWithTier(tierId, CancellationToken.None, true)).Returns(identities);
        var handler = CreateHandler(identitiesRepository, tierQuotaDefinitionsRepository);

        // Act
        await handler.Handle(new TierQuotaDefinitionCreatedDomainEvent(tierId, tierQuotaDefinition.Id));

        // Assert
        A.CallTo(() => identitiesRepository.Update(A<IEnumerable<Identity>>.That.Matches(ids =>
                ids.All(i => i.TierQuotas.Count == 1))
            , CancellationToken.None)
        ).MustHaveHappened();
    }

    // See comment in TierQuotaDefinitionCreatedDomainEventHandler about why this is commented out.
    // [Fact]
    // public async Task Updates_metric_statuses_after_creating_tier_quota()
    // {
    //     // Arrange
    //     var tierId = TierId.Parse("TIRFxoL0U24aUqZDSAWc");
    //
    //     var tierQuotaDefinition = new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 5, QuotaPeriod.Month);
    //     var tierQuotaDefinitionsRepository = new FindTierQuotaDefinitionsStubRepository(tierQuotaDefinition);
    //
    //     var firstIdentity = new Identity("some-identity-address-one", tierId);
    //     var secondIdentity = new Identity("some-identity-address-two", tierId);
    //     var identities = new List<Identity> { firstIdentity, secondIdentity };
    //     var identitiesRepository = A.Fake<IIdentitiesRepository>();
    //     A.CallTo(() => identitiesRepository.FindWithTier(tierId, CancellationToken.None, true)).Returns(identities);
    //     var metricStatusesService = A.Fake<IMetricStatusesService>();
    //     var handler = CreateHandler(identitiesRepository, tierQuotaDefinitionsRepository, metricStatusesService);
    //
    //     // Act
    //     await handler.Handle(new TierQuotaDefinitionCreatedDomainEvent(tierId, tierQuotaDefinition.Id));
    //
    //     // Assert
    //     A.CallTo(() => metricStatusesService.RecalculateMetricStatuses(
    //         A<List<string>>.That.Matches(x => x.Count == identities.Count),
    //         A<List<MetricKey>>.That.Contains(tierQuotaDefinition.MetricKey),
    //         A<CancellationToken>._)
    //     ).MustHaveHappened();
    // }

    private static TierQuotaDefinitionCreatedDomainEventHandler CreateHandler(IIdentitiesRepository identities, ITiersRepository tierQuotaDefinitions)
    {
        var logger = A.Fake<ILogger<TierQuotaDefinitionCreatedDomainEventHandler>>();
        return new TierQuotaDefinitionCreatedDomainEventHandler(identities, tierQuotaDefinitions, logger);
    }
}

using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Tiers.GetTierById;
public class HandlerTests
{
    [Fact]
    public async void Gets_tier_by_id()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var tierName = "some-tier-name";
        var tier = new Tier(tierId, tierName);

        var metricKey = MetricKey.NumberOfSentMessages;
        var max = 5;
        var period = QuotaPeriod.Month;
        tier.CreateQuota(metricKey, max, period);

        var stubTiersRepository = new FindTiersStubRepository(tier);
        var stubMetricsRepository = new FindMetricsStubRepository(new Metric(metricKey, "Number Of Sent Messages"));

        var handler = CreateHandler(stubTiersRepository, stubMetricsRepository);

        // Act
        var result = await handler.Handle(new GetTierByIdQuery(tierId), CancellationToken.None);

        // Assert
        result.Id.Should().Be(tierId);
        result.Name.Should().Be(tierName);
        result.Quotas.Should().HaveCount(1);
        result.Quotas.First().Max.Should().Be(max);
        result.Quotas.First().Period.Should().Be(period);
    }

    private Handler CreateHandler(ITiersRepository tiers, IMetricsRepository metrics)
    {
        return new Handler(tiers, metrics);
    }
}

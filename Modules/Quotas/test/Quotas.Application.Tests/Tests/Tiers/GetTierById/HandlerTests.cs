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
    public async void Gets_tier_by_id_one_quota()
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
        var stubMetricsRepository = new FindAllWithKeysMetricsStubRepository(new List<Metric> {new Metric(metricKey, "Number Of Sent Messages") });

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

    [Fact]
    public async void Gets_tier_by_id_multiple_quotas()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var tierName = "some-tier-name";
        var tier = new Tier(tierId, tierName);

        var metric1 = new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages");
        var metric2 = new Metric(MetricKey.FileStorageCapacity, "File Storage Capacity");
        var metric3 = new Metric(MetricKey.NumberOfRelationships, "Number Of Relationships");
        var metrics = new List<Metric> { metric1, metric2, metric3 };

        tier.CreateQuota(metric1.Key, 5, QuotaPeriod.Month);
        tier.CreateQuota(metric2.Key, 10, QuotaPeriod.Day);
        tier.CreateQuota(metric3.Key, 12, QuotaPeriod.Week);
        tier.CreateQuota(metric1.Key, 3, QuotaPeriod.Year);

        var stubTiersRepository = new FindTiersStubRepository(tier);
        var stubMetricsRepository = new FindAllWithKeysMetricsStubRepository(metrics);

        var handler = CreateHandler(stubTiersRepository, stubMetricsRepository);

        // Act
        var result = await handler.Handle(new GetTierByIdQuery(tierId), CancellationToken.None);

        // Assert
        result.Id.Should().Be(tierId);
        result.Name.Should().Be(tierName);
        result.Quotas.Should().HaveCount(4);

        foreach (var quota in result.Quotas)
        {
            if (quota.Metric.Key == metric1.Key.Value)
            {
                quota.Metric.DisplayName.Should().Be(metric1.DisplayName);
                quota.Max.Should().BeOneOf(5,3);
                quota.Period.Should().BeOneOf(QuotaPeriod.Month, QuotaPeriod.Year);
            }

            if (quota.Metric.Key == metric2.Key.Value)
            {
                quota.Metric.DisplayName.Should().Be(metric2.DisplayName);
                quota.Max.Should().Be(10);
                quota.Period.Should().Be(QuotaPeriod.Day);
            }

            if (quota.Metric.Key == metric3.Key.Value)
            {
                quota.Metric.DisplayName.Should().Be(metric3.DisplayName);
                quota.Max.Should().Be(12);
                quota.Period.Should().Be(QuotaPeriod.Week);
            }
        }
    }

    private Handler CreateHandler(ITiersRepository tiersRepository, IMetricsRepository metricsRepository)
    {
        return new Handler(tiersRepository, metricsRepository);
    }
}

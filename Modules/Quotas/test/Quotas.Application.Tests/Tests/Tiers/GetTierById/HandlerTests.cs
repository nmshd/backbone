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
    public async Task Gets_tier_by_id_one_quota()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        const string tierName = "some-tier-name";
        var tier = new Tier(tierId, tierName);

        var metricKey = MetricKey.NumberOfSentMessages;
        const int max = 5;
        const QuotaPeriod period = QuotaPeriod.Month;
        tier.CreateQuota(metricKey, max, period);

        var stubTiersRepository = new FindTiersStubRepository(tier);
        var stubMetricsRepository = new FindAllWithKeysMetricsStubRepository(new List<Metric> { new(metricKey, "Number Of Sent Messages") });

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
    public async Task Gets_tier_by_id_multiple_quotas()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        const string tierName = "some-tier-name";
        var tier = new Tier(tierId, tierName);

        var metricWithTwoQuotas = new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages");
        var metricWithOneQuota = new Metric(MetricKey.UsedFileStorageSpace, "Used File Storage Space");
        var metrics = new List<Metric> { metricWithTwoQuotas, metricWithOneQuota };

        tier.CreateQuota(metricWithTwoQuotas.Key, 1, QuotaPeriod.Day);
        tier.CreateQuota(metricWithOneQuota.Key, 1, QuotaPeriod.Day);
        tier.CreateQuota(metricWithTwoQuotas.Key, 5, QuotaPeriod.Week);

        var stubTiersRepository = new FindTiersStubRepository(tier);
        var stubMetricsRepository = new FindAllWithKeysMetricsStubRepository(metrics);

        var handler = CreateHandler(stubTiersRepository, stubMetricsRepository);

        // Act
        var result = await handler.Handle(new GetTierByIdQuery(tierId), CancellationToken.None);

        // Assert
        result.Id.Should().Be(tierId);
        result.Name.Should().Be(tierName);
        result.Quotas.Should().HaveCount(3);

        result.Quotas.ElementAt(0).Metric.Key.Should().Be(metricWithTwoQuotas.Key.Value);
        result.Quotas.ElementAt(0).Metric.DisplayName.Should().Be(metricWithTwoQuotas.DisplayName);
        result.Quotas.ElementAt(0).Max.Should().Be(1);
        result.Quotas.ElementAt(0).Period.Should().Be(QuotaPeriod.Day);

        result.Quotas.ElementAt(1).Metric.Key.Should().Be(metricWithOneQuota.Key.Value);
        result.Quotas.ElementAt(1).Metric.DisplayName.Should().Be(metricWithOneQuota.DisplayName);
        result.Quotas.ElementAt(1).Max.Should().Be(1);
        result.Quotas.ElementAt(1).Period.Should().Be(QuotaPeriod.Day);

        result.Quotas.ElementAt(2).Metric.Key.Should().Be(metricWithTwoQuotas.Key.Value);
        result.Quotas.ElementAt(2).Metric.DisplayName.Should().Be(metricWithTwoQuotas.DisplayName);
        result.Quotas.ElementAt(2).Max.Should().Be(5);
        result.Quotas.ElementAt(2).Period.Should().Be(QuotaPeriod.Week);
    }

    private Handler CreateHandler(ITiersRepository tiersRepository, IMetricsRepository metricsRepository)
    {
        return new Handler(tiersRepository, metricsRepository);
    }
}

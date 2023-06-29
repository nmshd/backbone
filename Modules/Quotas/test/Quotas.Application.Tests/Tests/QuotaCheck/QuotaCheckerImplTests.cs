using Backbone.Modules.Quotas.Application.QuotaCheck;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.QuotaCheck;
public class QuotaCheckerImplTests
{
    [Fact]
    public async Task ExhaustedStatuses_is_empty_when_the_metric_is_not_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImplementation(new MetricStatus(TestMetricKey, null));

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion(new[] { TestMetricKey });

        // Assert
        result.ExhaustedStatuses.Should().HaveCount(0);
    }

    [Fact]
    public async Task ExhaustedStatuses_has_value_when_one_metric_is_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImplementation(new MetricStatus(TestMetricKey, SystemTime.UtcNow.AddMinutes(10)));

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion(new[] { TestMetricKey });

        // Assert
        result.ExhaustedStatuses.Should().HaveCount(1);
    }

    [Fact]
    public async Task ExhaustedStatuses_has_values_when_several_metrics_are_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImplementation(
            new MetricStatus(TestMetricKey, SystemTime.UtcNow.AddMinutes(10)),
            new MetricStatus(AnotherTestMetricKey, SystemTime.UtcNow.AddMinutes(10))
            );

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion(new[] { TestMetricKey, AnotherTestMetricKey });

        // Assert
        result.ExhaustedStatuses.Should().HaveCount(2);
    }

    private static QuotaCheckerImpl CreateQuotaCheckerImplementation(params MetricStatus[] metricStatuses)
    {
        return new QuotaCheckerImpl(new UserContextStub(), new MetricStatusesStubRepository(metricStatuses.ToList()));
    }

    private static readonly MetricKey TestMetricKey = new("a-metric-key");
    private static readonly MetricKey AnotherTestMetricKey = new("another-metric-key");
}

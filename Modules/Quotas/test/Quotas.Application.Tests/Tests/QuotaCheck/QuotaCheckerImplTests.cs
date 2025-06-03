using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.QuotaCheck;

public class QuotaCheckerImplTests : AbstractTestsBase
{
    private static readonly MetricKey TEST_METRIC_KEY = new("a-metric-key");
    private static readonly MetricKey ANOTHER_TEST_METRIC_KEY = new("another-metric-key");

    [Fact]
    public async Task ExhaustedStatuses_is_empty_when_the_metric_is_not_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImpl(new MetricStatus(TEST_METRIC_KEY, null));

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion([TEST_METRIC_KEY]);

        // Assert
        result.ExhaustedStatuses.ShouldHaveCount(0);
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ExhaustedStatuses_has_value_when_one_metric_is_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImpl(new MetricStatus(TEST_METRIC_KEY, SystemTime.UtcNow.AddMinutes(10)));

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion([TEST_METRIC_KEY]);

        // Assert
        result.ExhaustedStatuses.ShouldHaveCount(1);
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task ExhaustedStatuses_has_values_when_several_metrics_are_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImpl(
            new MetricStatus(TEST_METRIC_KEY, SystemTime.UtcNow.AddMinutes(10)),
            new MetricStatus(ANOTHER_TEST_METRIC_KEY, SystemTime.UtcNow.AddMinutes(10))
        );

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion([TEST_METRIC_KEY, ANOTHER_TEST_METRIC_KEY]);

        // Assert
        result.ExhaustedStatuses.ShouldHaveCount(2);
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task ExhaustedStatuses_has_values_when_one_metric_is_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImpl(
            new MetricStatus(TEST_METRIC_KEY, SystemTime.UtcNow.AddMinutes(-10)),
            new MetricStatus(ANOTHER_TEST_METRIC_KEY, SystemTime.UtcNow.AddMinutes(10))
        );

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion([TEST_METRIC_KEY, ANOTHER_TEST_METRIC_KEY]);

        // Assert
        result.ExhaustedStatuses.ShouldHaveCount(1);
        result.ExhaustedStatuses.Single().MetricKey.ShouldBe(ANOTHER_TEST_METRIC_KEY);
        result.IsSuccess.ShouldBeFalse();
    }

    private static QuotaCheckerImpl CreateQuotaCheckerImpl(params MetricStatus[] metricStatuses)
    {
        return new QuotaCheckerImpl(new UserContextStub(), new MetricStatusesStubRepository(metricStatuses.ToList()));
    }
}

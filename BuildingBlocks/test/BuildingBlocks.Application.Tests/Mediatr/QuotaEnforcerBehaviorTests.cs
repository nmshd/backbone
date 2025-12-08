using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Attributes;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Domain;
using Backbone.UnitTestTools.Behaviors;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.Shouldly.Extensions;
using MediatR;

namespace Backbone.BuildingBlocks.Application.Tests.Mediatr;

public class QuotaEnforcerBehaviorTests : AbstractTestsBase
{
    [Fact]
    public async Task Calls_next_when_no_metric_is_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior();
        var nextMock = new NextMock<Unit>();

        // Act
        var acting = async () => await behavior.Handle(
            new TestCommand(),
            nextMock.Value,
            CancellationToken.None);

        await Task.Run(acting, CancellationToken.None);

        // Assert
        nextMock.WasCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task Throws_QuotaExhaustedException_when_exactly_one_metric_is_exhausted()
    {
        var exhaustionDate = DateTime.UtcNow.AddDays(1);
        var exhaustedMetricStatus = new MetricStatus(new MetricKey("exhausted"), exhaustionDate);

        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(exhaustedMetricStatuses: exhaustedMetricStatus);

        // Act
        var acting = async () => await behavior.Handle(
            new TestCommand(),
            new NextMock<Unit>().Value,
            CancellationToken.None
        );

        // Assert
        var exceptionExhaustedMetrics =
            (await acting.ShouldThrowAsync<QuotaExhaustedException>()).ExhaustedMetricStatuses;
        exceptionExhaustedMetrics.ShouldHaveCount(1);
        exceptionExhaustedMetrics.First().MetricKey.ShouldBe(new MetricKey("exhausted"));
        exceptionExhaustedMetrics.First().IsExhaustedUntil.ShouldBe(exhaustionDate);
    }

    [Fact]
    public async Task Thrown_QuotaExhaustedException_contains_information_about_each_exhausted_MetricStatus()
    {
        // Arrange
        var exhaustionDate1 = DateTime.UtcNow.AddDays(1);
        var exhaustionDate2 = DateTime.UtcNow.AddDays(10);
        var exhaustedMetricStatus1 = new MetricStatus(new MetricKey("exhausted1"), exhaustionDate1);
        var exhaustedMetricStatus2 = new MetricStatus(new MetricKey("exhausted2"), exhaustionDate2);
        var behavior = CreateQuotaEnforcerBehavior(exhaustedMetricStatuses: [exhaustedMetricStatus1, exhaustedMetricStatus2]
        );

        // Act
        var acting = async () => await behavior.Handle(
            new TestCommand(),
            new NextMock<Unit>().Value,
            CancellationToken.None);

        // Assert
        var exceptionExhaustedMetrics =
            (await acting.ShouldThrowAsync<QuotaExhaustedException>()).ExhaustedMetricStatuses;
        exceptionExhaustedMetrics.ShouldHaveCount(2);
        exceptionExhaustedMetrics.First().MetricKey.ShouldBe(new MetricKey("exhausted1"));
        exceptionExhaustedMetrics.First().IsExhaustedUntil.ShouldBe(exhaustionDate1);

        exceptionExhaustedMetrics.Second().MetricKey.ShouldBe(new MetricKey("exhausted2"));
        exceptionExhaustedMetrics.Second().IsExhaustedUntil.ShouldBe(exhaustionDate2);
    }

    private static QuotaEnforcerBehavior<TestCommand, Unit> CreateQuotaEnforcerBehavior(
        params MetricStatus[] exhaustedMetricStatuses)
    {
        return new QuotaEnforcerBehavior<TestCommand, Unit>(new QuotaCheckerStub(new CheckQuotaResult(exhaustedMetricStatuses)));
    }
}

internal class QuotaCheckerStub : IQuotaChecker
{
    private readonly CheckQuotaResult _expectedResult;

    public QuotaCheckerStub(CheckQuotaResult expectedResult)
    {
        _expectedResult = expectedResult;
    }

    public Task<CheckQuotaResult> CheckQuotaExhaustion(IEnumerable<MetricKey> metricKeys)
    {
        return Task.FromResult(_expectedResult);
    }
}

/// <summary>
/// The Metric key doesn't matter for these tests. The way the Mediatr Behavior is being
/// called does not inject the Metrics passed on the Attribute below. Tests will make use of
/// all the metrics available in the repository unless where specified.
/// </summary>
[ApplyQuotasForMetrics("DoesNotApplyToTests")]
internal class TestCommand : IRequest;

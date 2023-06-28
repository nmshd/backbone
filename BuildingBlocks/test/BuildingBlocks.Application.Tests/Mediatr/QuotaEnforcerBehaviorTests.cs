using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Application.MediatR;
using Enmeshed.BuildingBlocks.Domain;
using FluentAssertions;
using MediatR;
using Xunit;
using Enmeshed.UnitTestTools.Extensions;
using Enmeshed.UnitTestTools.Behaviors;
using Enmeshed.BuildingBlocks.Application.QuotaCheck;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;
public class QuotaEnforcerBehaviorTests
{
    [Fact]
    public async void Succeeds_when_the_metric_is_not_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior();
        var nextMock = new NextMock<TestData.IResponse>();

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestData.TestCommand(),
            nextMock.Value,
            CancellationToken.None);

        await Task.Run(acting, CancellationToken.None);

        // Assert
        nextMock.WasCalled.Should().BeTrue();
    }

    [Fact]
    public void Throws_QuotaExhaustedException_when_exactly_one_metric_is_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(TestData.MetricStatus.ThatIsExhaustedFor1Day);
        var nextMock = new NextMock<TestData.IResponse>();

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestData.TestCommand(),
            nextMock.Value,
            CancellationToken.None
        );

        // Assert
        var exceptionExhaustedMetrics = acting.Should().AwaitThrowAsync<QuotaExhaustedException>().Which.ExhaustedMetricStatuses;
        exceptionExhaustedMetrics.Should().HaveCount(1);
        exceptionExhaustedMetrics.First().MetricKey.Should().Be(TestData.MetricStatus.ThatIsExhaustedFor1Day.MetricKey);
    }

    [Fact]
    public void Throws_QuotaExhaustedException_when_more_than_one_metric_is_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(
            TestData.MetricStatus.ThatIsExhaustedFor1Day,
            TestData.MetricStatus.ThatIsExhaustedFor10Days
        );
        var nextMock = new NextMock<TestData.IResponse>();

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestData.TestCommand(),
            nextMock.Value,
            CancellationToken.None);

        // Assert
        var exceptionExhaustedMetrics = acting.Should().AwaitThrowAsync<QuotaExhaustedException>().Which.ExhaustedMetricStatuses;
        exceptionExhaustedMetrics.Should().HaveCount(2);
        exceptionExhaustedMetrics.All(it => it.IsExhaustedUntil > DateTime.Now).Should().BeTrue();
    }

    private static QuotaEnforcerBehavior<TestData.TestCommand, TestData.IResponse> CreateQuotaEnforcerBehavior(params MetricStatus[] metricStatuses)
    {
        return new QuotaEnforcerBehavior<TestData.TestCommand, TestData.IResponse>(new QuotaCheckerMock(new(metricStatuses)));
    }
}

internal class QuotaCheckerMock : IQuotaChecker
{
    private readonly CheckQuotaResult _expectedResult;

    public QuotaCheckerMock(CheckQuotaResult expectedResult)
    {
        _expectedResult = expectedResult;
    }

    public Task<CheckQuotaResult> CheckQuotaExhaustion(IEnumerable<MetricKey> metricKeys)
    {
        return Task.FromResult(_expectedResult);
    }
}

internal static class TestData
{
    /// <summary>
    /// The Metric key doesn't matter for these tests. The way the Mediatr Behavior is being
    /// called does not inject the Metrics passed on the Attribute below. Tests will make use of
    /// all the metrics available in the repository unless where specified.
    /// </summary>
    [ApplyQuotasForMetrics("DoesNotApplyToTests")]
    internal class TestCommand : IRequest { }

    internal class IResponse { }

    internal static class MetricStatus
    {
        public static readonly Domain.MetricStatus ThatIsExhaustedFor1Day = new(new MetricKey("ExhaustedFor1Day"), DateTime.Now.AddDays(1));

        public static readonly Domain.MetricStatus ThatIsExhaustedFor10Days = new(new MetricKey("ExhaustedFor10Days"), DateTime.Now.AddDays(10));
    }
}

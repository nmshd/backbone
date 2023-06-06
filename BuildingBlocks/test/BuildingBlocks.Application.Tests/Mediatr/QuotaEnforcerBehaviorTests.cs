using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Application.MediatR;
using Enmeshed.BuildingBlocks.Domain;
using FluentAssertions;
using MediatR;
using Xunit;
using Enmeshed.UnitTestTools.Extensions;
using Enmeshed.UnitTestTools.Behaviors;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;
public class QuotaEnforcerBehaviorTests
{
    [Fact]
    public async void Succeeds_when_the_metric_is_not_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(TestData.MetricStatus.ThatIsNotExhausted);
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
    public async void Succeeds_when_the_metric_was_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(TestData.MetricStatus.ThatWasExhaustedUntilYesterday);
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

    /// <summary>
    /// This test uses a special implementation of the IMetricStatusesRepository which <strong>
    /// returns zero results</strong> on the `GetMetricStatuses` Method.
    /// For this reason, the MetricStatuses passed to the constructor have zero relevance for the test.
    /// </summary>
    [Fact]
    public async void Succeeds_when_none_of_the_metrics_exist_in_the_repository()
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
    public async void Succeeds_when_none_of_the_metrics_are_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(
            TestData.MetricStatus.ThatWasExhaustedUntil2DaysAgo,
            TestData.MetricStatus.ThatWasExhaustedUntilYesterday
            );
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
    public void Throws_QuotaExhaustedException_when_the_metric_is_still_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(TestData.MetricStatus.ThatIsExhaustedFor10Days);
        var nextMock = new NextMock<TestData.IResponse>();

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestData.TestCommand(),
            nextMock.Value,
            CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<QuotaExhaustedException>()
            .Which.ExhaustedMetricStatuses.First().MetricKey.Should().Be(TestData.MetricStatus.ThatIsExhaustedFor10Days.MetricKey);
    }

    [Fact]
    public void Throws_QuotaExhaustedException_when_exactly_one_metric_is_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(
            TestData.MetricStatus.ThatIsExhaustedFor1Day,
            TestData.MetricStatus.ThatWasExhaustedUntilYesterday
            );
        var nextMock = new NextMock<TestData.IResponse>();

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestData.TestCommand(),
            nextMock.Value,
            CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<QuotaExhaustedException>()
            .Which.ExhaustedMetricStatuses.First().MetricKey.Should().Be(TestData.MetricStatus.ThatIsExhaustedFor1Day.MetricKey);
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
        acting.Should().AwaitThrowAsync<QuotaExhaustedException>()
            .Which.ExhaustedMetricStatuses.All(it =>
                it.IsExhaustedUntil > DateTime.Now
            ).Should().BeTrue();
    }

    private static QuotaEnforcerBehavior<TestData.TestCommand, TestData.IResponse> CreateQuotaEnforcerBehavior(params MetricStatus[] metricStatuses)
    {
        var metricStatusesRepository = new MetricStatusesStubRepository(metricStatuses.ToList());
        var userContextStub = new UserContextStub();
        return new QuotaEnforcerBehavior<TestData.TestCommand, TestData.IResponse>(metricStatusesRepository, userContextStub);
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

        public static readonly Domain.MetricStatus ThatWasExhaustedUntilYesterday = new(new MetricKey("ExhaustedUntilYesterday"), DateTime.Now.AddDays(-1));
        
        public static readonly Domain.MetricStatus ThatWasExhaustedUntil2DaysAgo = new(new MetricKey("ThatWasExhaustedUntil2DaysAgo"), DateTime.Now.AddDays(-2));

        public static readonly Domain.MetricStatus ThatIsNotExhausted = new(new MetricKey("ExhaustedUntilNull"), null);
    }
}

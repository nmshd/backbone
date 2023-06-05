using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Application.MediatR;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using FluentAssertions;
using MediatR;
using Xunit;
using static Enmeshed.BuildingBlocks.Application.Tests.Mediatr.TestData;
using Enmeshed.UnitTestTools.Extensions;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;
public class QuotaEnforcerBehaviorTests
{
    private static QuotaEnforcerBehavior<TestCommand, IResponse> CreateQuotaEnforcerBehavior(IMetricStatusesRepository metricStatusesRepository)
    {
        var userContextStub = new UserContextStub();
        return new QuotaEnforcerBehavior<TestCommand, IResponse>(metricStatusesRepository, userContextStub);
    }

    private static QuotaEnforcerBehavior<TestCommand, IResponse> CreateQuotaEnforcerBehavior(Domain.MetricStatus metricStatus)
    {
        var metricStatusesRepository = new MetricStatusesStubRepository(new() { metricStatus });
        var userContextStub = new UserContextStub();
        return new QuotaEnforcerBehavior<TestCommand, IResponse>(metricStatusesRepository, userContextStub);
    }

    [Fact]
    public async void Succeeds_when_the_passed_quota_is_not_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(TestData.MetricStatus.ThatIsNotExhausted);
        var nextHasRan = false;

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () =>
            {
                nextHasRan = true;
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        await Task.Run(acting, CancellationToken.None);

        // Assert
        nextHasRan.Should().BeTrue();
    }

    [Fact]
    public async void Succeeds_when_the_passed_quota_was_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(TestData.MetricStatus.ThatWasExhaustedUntilYesterday);
        var nextHasRan = false;

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () =>
            {
                nextHasRan = true;
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        await Task.Run(acting, CancellationToken.None);

        // Assert
        nextHasRan.Should().BeTrue();
    }

    [Fact]
    public void Throws_QuotaExhaustedException_when_the_passed_quota_is_still_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(TestData.MetricStatus.ThatIsExhaustedFor10Days);

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () =>
            {
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<QuotaExhaustedException>().Which.ExhaustedMetricStatuses.First().Should().Be(TestData.MetricStatus.ThatIsExhaustedFor10Days.MetricKey);
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
        var metricStatusesStubRepository = new MetricStatusesNoMatchStubRepository(new() {
            TestData.MetricStatus.ThatWasExhaustedUntilYesterday, TestData.MetricStatus.ThatIsNotExhausted
        });

        var behavior = CreateQuotaEnforcerBehavior(metricStatusesStubRepository);
        var nextHasRan = false;

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () =>
            {
                nextHasRan = true;
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        await Task.Run(acting, CancellationToken.None);

        // Assert
        nextHasRan.Should().BeTrue();
    }

    [Fact]
    public void Throws_QuotaExhaustedException_when_exactly_one_metric_is_exhausted()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            TestData.MetricStatus.ThatIsExhaustedFor1Day, TestData.MetricStatus.ThatWasExhaustedUntilYesterday
        });

        var behavior = CreateQuotaEnforcerBehavior(metricStatusesStubRepository);

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () =>
            {
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<QuotaExhaustedException>()
            .Which.ExhaustedMetricStatuses.First().Should().Be(TestData.MetricStatus.ThatIsExhaustedFor1Day.MetricKey);
    }

    [Fact]
    public void Throws_QuotaExhaustedException_when_more_than_one_metric_is_exhausted()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            TestData.MetricStatus.ThatIsExhaustedFor1Day, TestData.MetricStatus.ThatIsExhaustedFor10Days
        });

        var behavior = CreateQuotaEnforcerBehavior(metricStatusesStubRepository);

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () =>
            {
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<QuotaExhaustedException>()
            .Which.ExhaustedMetricStatuses.All(it =>
                it.IsExhaustedUntil > DateTime.Now
            ).Should().BeTrue();
    }

    [Fact]
    public async void Succeeds_when_none_of_the_passed_quotas_are_exhausted()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            TestData.MetricStatus.ThatWasExhaustedUntil2DaysAgo, TestData.MetricStatus.ThatWasExhaustedUntilYesterday
        });

        var behavior = CreateQuotaEnforcerBehavior(metricStatusesStubRepository);
        var nextHasRan = false;

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () =>
            {
                nextHasRan = true;
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        await Task.Run(acting, CancellationToken.None);

        // Assert
        nextHasRan.Should().BeTrue();
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
        public static readonly Domain.MetricStatus ThatIsExhaustedFor1Day = new(new MetricKey("ExhaustedUntilTomorrow"), DateTime.Now.AddDays(1));

        public static readonly Domain.MetricStatus ThatIsExhaustedFor10Days = new(new MetricKey("ExhaustedFor10Days"), DateTime.Now.AddDays(10));

        public static readonly Domain.MetricStatus ThatWasExhaustedUntilYesterday = new(new MetricKey("ExhaustedUntilYesterday"), DateTime.Now.AddDays(-1));
        
        public static readonly Domain.MetricStatus ThatWasExhaustedUntil2DaysAgo = new(new MetricKey("ThatWasExhaustedUntil2DaysAgo"), DateTime.Now.AddDays(-2));

        public static readonly Domain.MetricStatus ThatIsNotExhausted = new(new MetricKey("ExhaustedUntilNull"), null);
    }
}

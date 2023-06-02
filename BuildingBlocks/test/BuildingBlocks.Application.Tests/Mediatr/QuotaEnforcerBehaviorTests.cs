using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Application.MediatR;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using FluentAssertions;
using MediatR;
using Xunit;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;
public class QuotaEnforcerBehaviorTests
{
    private static QuotaEnforcerBehavior<TestData.TestCommand, TestData.IResponse> CreateQuotaEnforcerBehavior(IMetricStatusesRepository metricStatusesRepository)
    {
        var userContextStub = new UserContextStub();
        return new QuotaEnforcerBehavior<TestData.TestCommand, TestData.IResponse>(metricStatusesRepository, userContextStub);
    }

    [Fact]
    public async void Throws_QuotaExhaustedException_when_at_least_one_metric_is_exhausted()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            TestData.MetricStatus.ThatIsExhaustedFor1Day, TestData.MetricStatus.ThatWasExhaustedUntilYesterday
        });

        var behavior = CreateQuotaEnforcerBehavior(metricStatusesStubRepository);

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestData.TestCommand(),
            () =>
            {
                return Task.FromResult(new TestData.IResponse());
            },
            CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<QuotaExhaustedException>();
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

        public static readonly Domain.MetricStatus ThatIsNotExhausted = new(new MetricKey("ExhaustedUntilNull"), null);
    }
}

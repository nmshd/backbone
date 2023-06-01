using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Application.MediatR;
using Enmeshed.BuildingBlocks.Domain;
using FluentAssertions;
using MediatR;
using Xunit;

using DomainMetricStatus = Enmeshed.BuildingBlocks.Domain.MetricStatus;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;
public class QuotaEnforcerBehaviorTests
{
    private readonly IUserContext _userContextStub;

    public QuotaEnforcerBehaviorTests() {
        _userContextStub = new UserContextStub();
    }

    [ApplyQuotasForMetrics("*")]
    private class TestCommand : IRequest { }

    private class IResponse { }

    [Fact]
    public async void Throws_QuotaExhaustedException_when_at_least_one_metric_is_exhausted()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            TestData.MetricStatus.ThatIsExhaustedFor1Day, TestData.MetricStatus.ThatWasExhaustedUntilYesterday
        });

        var behavior = new QuotaEnforcerBehavior<TestCommand, IResponse>(metricStatusesStubRepository, _userContextStub);

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () => {
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<QuotaExhaustedException>();
    }

    [Fact]
    public void Throws_QuotaExhaustedException_when_at_least_one_metric_is_exhausted_with_farthest_quota_on_data()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            TestData.MetricStatus.ThatIsExhaustedFor1Day, TestData.MetricStatus.ThatIsExhaustedFor10Days
        });

        var behavior = new QuotaEnforcerBehavior<TestCommand, IResponse>(metricStatusesStubRepository, _userContextStub);

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () => {
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<QuotaExhaustedException>().Which.MetricKey.Should().Be(TestData.MetricStatus.ThatIsExhaustedFor10Days.MetricKey);
    }

    [Fact]
    public async void Succeeds_when_no_exhausted_quotas_exist()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            TestData.MetricStatus.ThatWasExhaustedUntilYesterday, TestData.MetricStatus.ThatIsNotExhausted
        });
        
        var behavior = new QuotaEnforcerBehavior<TestCommand, IResponse>(metricStatusesStubRepository, _userContextStub);
        var nextHasRan = false;

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () => { 
                nextHasRan = true;
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        await Task.Run(acting, CancellationToken.None);

        // Assert
        nextHasRan.Should().BeTrue();
    }
}

public static class TestData
{
    public static class MetricStatus
    {
        public static readonly DomainMetricStatus ThatIsExhaustedFor1Day = new(new MetricKey("ExhaustedUntilTomorrow"), DateTime.Now.AddDays(1));

        public static readonly DomainMetricStatus ThatIsExhaustedFor10Days = new(new MetricKey("ExhaustedFor10Days"), DateTime.Now.AddDays(10));

        public static readonly DomainMetricStatus ThatWasExhaustedUntilYesterday = new (new MetricKey("ExhaustedUntilYesterday"), DateTime.Now.AddDays(-1));

        public static readonly DomainMetricStatus ThatIsNotExhausted = new (new MetricKey("ExhaustedUntilNull"), null);
    }
}

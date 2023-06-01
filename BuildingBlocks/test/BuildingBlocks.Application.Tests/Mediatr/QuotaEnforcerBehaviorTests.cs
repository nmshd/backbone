using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Application.MediatR;
using Enmeshed.BuildingBlocks.Domain;
using FluentAssertions;
using MediatR;
using Xunit;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;
public class QuotaEnforcerBehaviorTests
{
    private readonly IUserContext _userContextStub;

    public QuotaEnforcerBehaviorTests() {
        _userContextStub = new UserContextStub();
    }

    [ApplyQuotasForMetrics("KeyOne, KeyTwo")]
    private class TestCommand : IRequest { }

    private class IResponse { }

    [Fact]
    public async void Throws_QuotaExhaustedException_when_at_least_one_metric_is_exhausted()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            new MetricStatus(new MetricKey("KeyOne"), DateTime.UtcNow.AddDays(1)) ,
            new MetricStatus(new MetricKey("KeyTwo"), DateTime.UtcNow.AddDays(1))
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
        var expectedMetricStatus = new MetricStatus(new MetricKey("KeyTwo"), DateTime.UtcNow.AddDays(10));
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            new MetricStatus(new MetricKey("KeyOne"), DateTime.UtcNow.AddDays(1)),
            expectedMetricStatus
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
        acting.Should().AwaitThrowAsync<QuotaExhaustedException>().Which.MetricKey.Should().Be(expectedMetricStatus.MetricKey);
    }

    [Fact]
    public async void Runs_when_no_quotas_have_been_exceeded()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            new MetricStatus(new MetricKey("KeyOne"), DateTime.UtcNow.AddDays(-1)) ,
            new MetricStatus(new MetricKey("KeyTwo"), DateTime.UtcNow.AddDays(-1))
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

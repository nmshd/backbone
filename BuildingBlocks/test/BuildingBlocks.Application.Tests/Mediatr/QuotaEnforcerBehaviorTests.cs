using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Application.MediatR;
using Enmeshed.BuildingBlocks.Domain;
using FluentAssertions;
using MediatR;
using Xunit;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;
public class QuotaEnforcerBehaviorTests
{
    public QuotaEnforcerBehaviorTests() { }

    [ApplyQuotasForMetrics("MetricKey")]
    private class TestCommand : IRequest<IResponse> { }

    private class IResponse { }

    [Fact]
    public async void Throws_QuotaExhaustedException_OnExhaustedQuota()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            new MetricStatus(new MetricKey("KeyOne"), DateTime.UtcNow.AddDays(1)) 
        });

        var userContextStub = new UserContextStub();
        var behavior = new QuotaEnforcerBehavior<TestCommand, IResponse>(metricStatusesStubRepository, userContextStub);

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
    public async void Throws_QuotaExhaustedException_OnExhaustedQuota_WithFarthestQuotaOnData()
    {
        // Arrange
        var expectedMetricStatus = new MetricStatus(new MetricKey("KeyTwo"), DateTime.UtcNow.AddDays(10));
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            new MetricStatus(new MetricKey("KeyOne"), DateTime.UtcNow.AddDays(1)),
            expectedMetricStatus
        });

        var userContextStub = new UserContextStub();
        var behavior = new QuotaEnforcerBehavior<TestCommand, IResponse>(metricStatusesStubRepository, userContextStub);

        // Act
        Func<Task> action = async () => await behavior.Handle(
            new TestCommand(),
            () => {
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<QuotaExhaustedException>(action);
        ex.MetricKey.Should().Be(expectedMetricStatus.MetricKey);
    }

    [Fact]
    public async void Runs_WithoutExceptions()
    {
        // Arrange
        var metricStatusesStubRepository = new MetricStatusesStubRepository(new() {
            new MetricStatus(new MetricKey("KeyOne"), DateTime.UtcNow.AddDays(-1))
        });
        
        var userContextStub = new UserContextStub();
        var behavior = new QuotaEnforcerBehavior<TestCommand, IResponse>(metricStatusesStubRepository, userContextStub);
        var nextHasRan = false;

        // Act
        Func<Task> action = async () => await behavior.Handle(
            new TestCommand(),
            () => { 
                nextHasRan = true;
                return Task.FromResult(new IResponse());
            },
            CancellationToken.None);

        await Task.Run(action, CancellationToken.None);

        // Assert
        nextHasRan.Should().BeTrue();
    }
}

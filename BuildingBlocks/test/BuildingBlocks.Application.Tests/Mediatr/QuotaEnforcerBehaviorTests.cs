using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Application.MediatR;
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
    public async void Returns_()
    {
        // Arrange
        var metricStatusesRepositoriesMock = new MockMetricStatusesRepository();
        var userContextMock = new UserContextMock();

        var behavior = new QuotaEnforcerBehavior<TestCommand, IResponse>(metricStatusesRepositoriesMock, userContextMock);

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            () => { return Task.FromResult(new IResponse()); },
            CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<QuotaExhaustedException>();
    }
}

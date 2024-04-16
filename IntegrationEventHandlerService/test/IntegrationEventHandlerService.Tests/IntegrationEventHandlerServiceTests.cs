using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using FakeItEasy;

namespace Backbone.IntegrationEventHandlerService.Tests;

public class IntegrationEventHandlerServiceTests
{
    [Fact]
    public async void HappyPath()
    {
        // Arrange
        var mockEventBus = A.Fake<IEventBus>();

        var integrationEventService = CreateService(mockEventBus);

        // Act
        await integrationEventService.StartAsync(CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.StartConsuming()).MustHaveHappenedOnceExactly();

    }

    private static IntegrationEventHandlerService CreateService(IEventBus? eventBus = null)
    {
        eventBus ??= A.Fake<IEventBus>();

        return new IntegrationEventHandlerService(
            eventBus,
            A.Dummy<IEnumerable<AbstractModule>>());
    }
}

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

        var integrationEventHandlerService = CreateService(mockEventBus);

        // Act
        await integrationEventHandlerService.StartAsync(CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.StartConsuming()).MustHaveHappenedOnceExactly();

    }

    private static IntegrationEventHandlerService CreateService(IEventBus eventBus)
    {
        return new IntegrationEventHandlerService(
            eventBus,
            A.Dummy<IEnumerable<AbstractModule>>());
    }
}

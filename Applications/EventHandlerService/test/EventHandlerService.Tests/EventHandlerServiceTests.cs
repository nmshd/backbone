using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.EventHandlerService.Tests;

public class EventHandlerServiceTests : AbstractTestsBase
{
    [Fact]
    public async Task HappyPath()
    {
        // Arrange
        var mockEventBus = A.Fake<IEventBus>();

        var eventHandlerService = CreateService(mockEventBus);

        // Act
        await eventHandlerService.StartAsync(CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.StartConsuming(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static EventHandlerService CreateService(IEventBus eventBus)
    {
        return new EventHandlerService(
            eventBus,
            A.Dummy<IEnumerable<IEventBusConfigurator>>(),
            A.Dummy<ILogger<EventHandlerService>>());
    }
}

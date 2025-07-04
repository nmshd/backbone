using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.SseServer;
using Backbone.SseServer.Controllers;
using Backbone.Tooling.Extensions;
using Backbone.UnitTestTools.Shouldly.Extensions;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.SseServerTests;

public class EventQueueTests : AbstractTestsBase
{
    [Fact]
    public void Cannot_register_if_already_registered()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>(), A.Fake<IOptions<Configuration>>());
        queue.Register("testAddress", CancellationToken.None);

        // Act
        var acting = () => queue.Register("testAddress", CancellationToken.None);

        // Assert
        acting.ShouldThrow<ClientAlreadyRegisteredException>();
    }

    [Fact]
    public void Can_deregister_even_if_not_registered()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>(), A.Fake<IOptions<Configuration>>());

        // Act
        var acting = () => queue.Deregister("testAddress");

        // Assert
        acting.ShouldNotThrow();
    }

    [Fact]
    public async Task Cannot_enqueue_for_identity_that_is_not_registered()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>(), A.Fake<IOptions<Configuration>>());

        // Act
        var acting = () => queue.EnqueueFor("testAddress", "testEventName", CancellationToken.None);

        // Assert
        await acting.ShouldThrowAsync<ClientNotFoundException>();
    }

    [Fact]
    public void Cannot_dequeue_for_identity_that_is_not_registered()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>(), A.Fake<IOptions<Configuration>>());

        // Act
        var acting = () => queue.DequeueFor("testAddress", CancellationToken.None);

        // Assert
        acting.ShouldThrow<Exception>();
    }

    [Fact]
    public async Task Dequeue_returns_elements_from_the_queue()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>(), A.Fake<IOptions<Configuration>>());
        queue.Register("testAddress", CancellationToken.None);

        await queue.EnqueueFor("testAddress", "event1", CancellationToken.None);
        await queue.EnqueueFor("testAddress", "event2", CancellationToken.None);

        // Act
        var eventNames = await queue.DequeueAllFor("testAddress");

        // Assert
        eventNames.ShouldHaveCount(2);
        eventNames.ShouldContain("event1");
        eventNames.ShouldContain("event2");
    }

    [Fact]
    public async Task Dequeue_only_returns_elements_for_the_given_identity()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>(), A.Fake<IOptions<Configuration>>());
        queue.Register("testAddress1", CancellationToken.None);
        queue.Register("testAddress2", CancellationToken.None);

        await queue.EnqueueFor("testAddress1", "event1", CancellationToken.None);
        await queue.EnqueueFor("testAddress2", "event2", CancellationToken.None);

        // Act
        var eventNames = await queue.DequeueAllFor("testAddress1");

        // Assert
        eventNames.ShouldHaveCount(1);
        eventNames.ShouldContain("event1");
    }

    [Fact]
    public async Task Queue_sends_keep_alive_event()
    {
        // Arrange
        var config = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration(),
            SseServer = new Configuration.SseServerConfiguration { KeepAliveEventIntervalInSeconds = 1 },
            Cors = new Configuration.CorsConfiguration(),
            Infrastructure = new Configuration.InfrastructureConfiguration { EventBus = new EventBusConfiguration { ProductName = "AzureServiceBus" } }
        };
        var options = new OptionsWrapper<Configuration>(config);

        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>(), options);
        queue.Register("testAddress", CancellationToken.None);

        // Act
        await Task.Delay(1.Seconds(), TestContext.Current.CancellationToken);

        // Assert
        var eventNames = await queue.DequeueAllFor("testAddress");
        eventNames.Count.ShouldBeGreaterThanOrEqualTo(1);
        eventNames.ShouldContain(EventQueue.KEEP_ALIVE_EVENT);
    }
}

file static class Extensions
{
    public static async Task<List<string>> DequeueAllFor(this EventQueue queue, string address)
    {
        var cancellationToken = new CancellationTokenSource();
        cancellationToken.CancelAfter(TimeSpan.FromSeconds(1));

        var eventNames = new List<string>();
        try
        {
            await foreach (var eventName in queue.DequeueFor(address, cancellationToken.Token))
            {
                eventNames.Add(eventName);
            }
        }
        catch (OperationCanceledException)
        {
        }

        return eventNames;
    }
}

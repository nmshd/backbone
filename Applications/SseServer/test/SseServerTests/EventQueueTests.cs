using Backbone.SseServer.Controllers;
using Backbone.UnitTestTools.Shouldly.Extensions;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.SseServerTests;

public class EventQueueTests : AbstractTestsBase
{
    [Fact]
    public void Cannot_register_if_already_registered()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>());
        queue.Register("testAddress");

        // Act
        var acting = () => queue.Register("testAddress");

        // Assert
        acting.ShouldThrow<ClientAlreadyRegisteredException>();
    }

    [Fact]
    public void Can_deregister_even_if_not_registered()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>());

        // Act
        var acting = () => queue.Deregister("testAddress");

        // Assert
        acting.ShouldNotThrow();
    }

    [Fact]
    public async Task Cannot_enqueue_for_identity_that_is_not_registered()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>());

        // Act
        var acting = () => queue.EnqueueFor("testAddress", "testEventName", CancellationToken.None);

        // Assert
        await acting.ShouldThrowAsync<ClientNotFoundException>();
    }

    [Fact]
    public void Cannot_dequeue_for_identity_that_is_not_registered()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>());

        // Act
        var acting = () => queue.DequeueFor("testAddress", CancellationToken.None);

        // Assert
        acting.ShouldThrow<Exception>();
    }

    [Fact]
    public async Task Dequeue_returns_elements_from_the_queue()
    {
        // Arrange
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>());
        queue.Register("testAddress");

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
        var queue = new EventQueue(A.Fake<ILogger<EventQueue>>());
        queue.Register("testAddress1");
        queue.Register("testAddress2");

        await queue.EnqueueFor("testAddress1", "event1", CancellationToken.None);
        await queue.EnqueueFor("testAddress2", "event2", CancellationToken.None);

        // Act
        var eventNames = await queue.DequeueAllFor("testAddress1");

        // Assert
        eventNames.ShouldHaveCount(1);
        eventNames.ShouldContain("event1");
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

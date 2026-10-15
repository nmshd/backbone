using System.Diagnostics.Metrics;
using System.Text.Json;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.Outbox;

public class RabbitMqOutboxCompatibilityTests : AbstractTestsBase
{
    [Fact]
    public async Task Persisted_contract_is_published_with_confirms_and_consumed_without_payload_changes()
    {
        var connection = Environment.GetEnvironmentVariable("BACKBONE_OUTBOX_TEST_RABBITMQ");
        Assert.SkipWhen(string.IsNullOrEmpty(connection), "No dedicated RabbitMQ test connection configured.");
        var services = new ServiceCollection();
        services.AddSingleton<CaptureHandler>();
        await using var container = services.BuildServiceProvider();
        using var meter = new Meter("outbox-compatibility-test");
        using var bus = await EventBusRabbitMq.Create(new ConnectionFactory { Uri = new Uri(connection!) }, NullLogger<EventBusRabbitMq>.Instance,
            container, "outbox-compatibility-tests", new EventBusMetrics(meter));
        await bus.Subscribe<RabbitMqOutboxCompatibilityDomainEvent, CaptureHandler>();
        await bus.StartConsuming(TestContext.Current.CancellationToken);
        try
        {
            var original = new RabbitMqOutboxCompatibilityDomainEvent { Name = "contract" };
            var row = OutboxMessage.Create(original, "test-stream");
            await bus.Publish(JsonSerializer.Deserialize<RabbitMqOutboxCompatibilityDomainEvent>(row.Payload)!);
            var received = await container.GetRequiredService<CaptureHandler>().Received.Task.WaitAsync(TimeSpan.FromSeconds(15), TestContext.Current.CancellationToken);
            received.DomainEventId.ShouldBe(original.DomainEventId);
            received.CreationDate.ShouldBe(original.CreationDate);
            received.Name.ShouldBe(original.Name);
        }
        finally
        {
            await bus.StopConsuming(CancellationToken.None);
        }
    }
}

public class CaptureHandler : IDomainEventHandler<RabbitMqOutboxCompatibilityDomainEvent>
{
    public TaskCompletionSource<RabbitMqOutboxCompatibilityDomainEvent> Received { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Task Handle(RabbitMqOutboxCompatibilityDomainEvent domainEvent)
    {
        Received.TrySetResult(domainEvent);
        return Task.CompletedTask;
    }
}

public class RabbitMqOutboxCompatibilityDomainEvent : DomainEvent
{
    public string Name { get; set; } = "";
}

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Microsoft.EntityFrameworkCore;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public class OutboxMessage
{
    private static long _sqliteSequence = DateTime.UtcNow.Ticks;
    public static OutboxMessage Create(DomainEvent domainEvent, string stream, string? provider = null) => new()
    {
        Id = Guid.NewGuid(),
        Sequence = provider == "Microsoft.EntityFrameworkCore.Sqlite" ? Interlocked.Increment(ref _sqliteSequence) : 0,
        EventId = domainEvent.DomainEventId,
        EventName = domainEvent.GetEventName(),
        EventType = domainEvent.GetType().FullName + ", " + domainEvent.GetType().Assembly.GetName().Name,
        Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
        Stream = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(stream))),
        CreatedAt = DateTime.UtcNow,
        NextAttemptAt = DateTime.UtcNow,
        CorrelationId = CustomLogContext.GetCorrelationId(),
        TraceParent = Activity.Current?.Id,
        TraceState = Activity.Current?.TraceStateString
    };

    public Guid Id { get; set; }
    public long Sequence { get; set; }
    public Guid SaveId { get; set; }
    public int Position { get; set; }
    public string EventId { get; set; } = "";
    public string EventName { get; set; } = "";
    public string EventType { get; set; } = "";
    public string Payload { get; set; } = "";
    public string Stream { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int Attempts { get; set; }
    public DateTime NextAttemptAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? LastError { get; set; }
    public Guid? LeaseId { get; set; }
    public DateTime? LeaseExpiresAt { get; set; }
    public string CorrelationId { get; set; } = "";
    public string? TraceParent { get; set; }
    public string? TraceState { get; set; }
}

public class InboxMessage
{
    public string EventId { get; set; } = "";
    public string Handler { get; set; } = "";
    public DateTime ProcessedAt { get; set; }
}

public static class OutboxDbContextExtensions
{
    // For handler fan-out that has no aggregate mutation; caller saves in its local transaction.
    public static void AddOutboxEvent(this DbContext context, DomainEvent domainEvent, string stream)
    {
        context.Add(OutboxMessage.Create(domainEvent, stream, context.Database.ProviderName));
    }
}

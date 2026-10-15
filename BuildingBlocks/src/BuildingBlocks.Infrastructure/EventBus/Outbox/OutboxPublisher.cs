using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public sealed class OutboxPublisher(
    IEnumerable<OutboxDatabase> databases,
    OutboxStore store,
    IEventBus eventBus,
    IOptions<OutboxOptions> options,
    OutboxDiagnostics metrics,
    ILogger<OutboxPublisher> logger) : BackgroundService
{
    private static readonly ConcurrentDictionary<string, Type> EVENT_TYPES = new();
    private readonly OutboxOptions _options = options.Value;

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.WhenAll(databases.Select(database => Run(database, stoppingToken)));

    private async Task Run(OutboxDatabase database, CancellationToken token)
    {
        var nextMetricsAt = DateTime.MinValue;
        while (!token.IsCancellationRequested)
        {
            try
            {
                var count = await PublishBatch(database, token);
                if (DateTime.UtcNow >= nextMetricsAt)
                {
                    var (pending, age) = await store.Backlog(database, token);
                    metrics.Backlog(database.Module, pending, age);
                    nextMetricsAt = DateTime.UtcNow.AddSeconds(30);
                }
                if (count != 0)
                    continue;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { break; }
            catch (Exception error)
            {
                // Exception messages may contain payloads or connection strings; log only the error type.
                logger.LogError("Outbox polling failed for {Module}: {ErrorType}.", database.Module, error.GetType().Name);
            }
            try { await Task.Delay(_options.PollIntervalMilliseconds, token); }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { break; }
        }
    }

    public async Task<int> PublishBatch(OutboxDatabase database, CancellationToken token)
    {
        var lease = Guid.NewGuid();
        var messages = await store.Claim(database, _options.BatchSize, lease, _options.LeaseSeconds, token);
        if (messages.Count == 0)
            return 0;

        using var renewal = CancellationTokenSource.CreateLinkedTokenSource(token);
        var heartbeat = Renew(database, lease, renewal);
        try
        {
            await Parallel.ForEachAsync(messages, new ParallelOptions { MaxDegreeOfParallelism = _options.Parallelism, CancellationToken = renewal.Token },
                async (message, cancellationToken) => await Publish(database, message, lease, cancellationToken));
        }
        finally
        {
            await renewal.CancelAsync();
            await heartbeat;
        }
        return messages.Count;
    }

    private async Task Renew(OutboxDatabase database, Guid lease, CancellationTokenSource renewal)
    {
        try
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(_options.LeaseSeconds / 3.0), renewal.Token);
                if (await store.Renew(database, lease, _options.LeaseSeconds, renewal.Token) == 0)
                    return;
            }
        }
        catch (OperationCanceledException) when (renewal.IsCancellationRequested) { }
        catch (Exception error)
        {
            logger.LogError("Outbox lease renewal failed for {Module}: {ErrorType}.", database.Module, error.GetType().Name);
            await renewal.CancelAsync();
        }
    }

    private async Task Publish(OutboxDatabase database, OutboxMessage message, Guid lease, CancellationToken token)
    {
        if (await store.Own(database, message.Id, lease, _options.LeaseSeconds, token) != 1)
            return;
        using var correlation = CustomLogContext.SetCorrelationId(message.CorrelationId);
        ActivityContext.TryParse(message.TraceParent, message.TraceState, isRemote: true, out var parent);
        // Activity is started even without an OTel listener, so providers can propagate the saved parent.
        using var propagationActivity = new Activity("outbox.publish").SetIdFormat(ActivityIdFormat.W3C);
        if (parent != default)
            propagationActivity.SetParentId(parent.TraceId, parent.SpanId, parent.TraceFlags);
        propagationActivity.TraceStateString = message.TraceState;
        propagationActivity.Start();
        using var activity = OutboxDiagnostics.ACTIVITY_SOURCE.StartActivity("outbox.publish", ActivityKind.Producer);
        activity?.SetTag("outbox.message.id", message.Id.ToString());
        var tag = new KeyValuePair<string, object?>("module", database.Module);
        var started = Stopwatch.GetTimestamp();
        if (message.Attempts > 1)
            metrics.Retries.Add(1, tag);
        try
        {
            if (message.Attempts > _options.MaxAttempts)
                throw new InvalidOperationException("Outbox attempt limit reached after an interrupted attempt.");
            var eventType = EVENT_TYPES.GetOrAdd(message.EventType, name =>
            {
                var type = Type.GetType(name, throwOnError: true)!;
                if (!type.IsSubclassOf(typeof(DomainEvent)) || type.IsAbstract)
                    throw new InvalidOperationException("Outbox type is not a concrete domain event.");
                return type;
            });
            if (eventType.GetEventName() != message.EventName)
                throw new InvalidOperationException("Outbox event name no longer matches its contract.");
            var domainEvent = (DomainEvent)JsonSerializer.Deserialize(message.Payload, eventType)!;
            await eventBus.Publish(domainEvent);
            // A shutdown after broker acknowledgement deliberately leaves the lease for redelivery.
            if (await store.Complete(database, message.Id, lease, token) == 1)
            {
                metrics.Published.Add(1, tag);
                metrics.DeliveryDelay.Record(Math.Max(0, (DateTime.UtcNow - message.CreatedAt).TotalSeconds), tag);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
        catch (Exception error)
        {
            metrics.Failed.Add(1, tag);
            activity?.SetStatus(ActivityStatusCode.Error, error.GetType().Name);
            logger.LogWarning("Outbox publication {OutboxId} failed for {Module} on attempt {Attempt}: {ErrorType}.", message.Id, database.Module, message.Attempts, error.GetType().Name);
            await store.Fail(database, message, lease, _options, error, token);
        }
        finally
        {
            metrics.PublishingDuration.Record(Stopwatch.GetElapsedTime(started).TotalSeconds, tag);
        }
    }
}

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public sealed class OutboxDiagnostics : IDisposable
{
    public const string NAME = "enmeshed.backbone.outbox";
    private readonly Meter _meter = new(NAME);
    public static readonly ActivitySource ACTIVITY_SOURCE = new(NAME);
    private readonly ConcurrentDictionary<string, (long Count, double Age)> _backlogs = new();
    public Counter<long> Published { get; }
    public Counter<long> Failed { get; }
    public Counter<long> Retries { get; }
    public Histogram<double> PublishingDuration { get; }
    public Histogram<double> DeliveryDelay { get; }

    public OutboxDiagnostics()
    {
        Published = _meter.CreateCounter<long>(NAME + ".published");
        Failed = _meter.CreateCounter<long>(NAME + ".failed");
        Retries = _meter.CreateCounter<long>(NAME + ".retries");
        PublishingDuration = _meter.CreateHistogram<double>(NAME + ".publishing.duration", "s");
        DeliveryDelay = _meter.CreateHistogram<double>(NAME + ".delivery.delay", "s");
        _meter.CreateObservableGauge(NAME + ".pending", () => _backlogs.Select(x => new Measurement<long>(x.Value.Count, new KeyValuePair<string, object?>("module", x.Key))));
        _meter.CreateObservableGauge(NAME + ".oldest.age", () => _backlogs.Select(x => new Measurement<double>(x.Value.Age, new KeyValuePair<string, object?>("module", x.Key))), "s");
    }

    public void Backlog(string module, long count, double age) => _backlogs[module] = (count, age);
    public void Dispose() => _meter.Dispose();
}

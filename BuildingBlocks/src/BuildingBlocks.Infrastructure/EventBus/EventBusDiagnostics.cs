using System.Diagnostics;
using OpenTelemetry.Context.Propagation;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public class EventBusDiagnostics
{
    public const string ACTIVITY_SOURCE_NAME = "Backbone.EventBus";

    public static readonly ActivitySource ACTIVITY_SOURCE = new(ACTIVITY_SOURCE_NAME);
    public static readonly TextMapPropagator PROPAGATOR = Propagators.DefaultTextMapPropagator;
}

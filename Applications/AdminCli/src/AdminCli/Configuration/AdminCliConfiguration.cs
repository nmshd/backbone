using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.AdminCli.Configuration;

public class AdminCliConfiguration
{
    [Required]
    public required TelemetryConfiguration Telemetry { get; init; }

    [Required]
    public required AdminInfrastructureConfiguration Infrastructure { get; init; }

    public class TelemetryConfiguration
    {
        public required OpenTelemetryCollectorConfiguration OpenTelemetryCollector { get; set; } = new();
    }

    public class AdminInfrastructureConfiguration
    {
        [Required]
        public required EventBusConfiguration EventBus { get; init; }

        [Required]
        public required DatabaseConfiguration SqlDatabase { get; init; }
    }
}

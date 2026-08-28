using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.DatabaseMigrator;

public class Configuration
{
    [Required]
    public required TelemetryConfiguration Telemetry { get; init; }

    [Required]
    public required InfrastructureConfiguration Infrastructure { get; init; }
}

public class TelemetryConfiguration
{
    public required OpenTelemetryCollectorConfiguration OpenTelemetryCollector { get; set; } = new();
}

public class InfrastructureConfiguration
{
    [Required]
    public required DatabaseConfiguration SqlDatabase { get; init; }
}

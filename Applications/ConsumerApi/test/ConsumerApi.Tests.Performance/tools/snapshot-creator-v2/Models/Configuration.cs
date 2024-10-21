using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record Configuration
{
    [JsonPropertyName(nameof(App))] public AppConfig App { get; set; } = null!;

    [JsonPropertyName(nameof(Connector))] public ConnectorConfig Connector { get; set; } = null!;
}

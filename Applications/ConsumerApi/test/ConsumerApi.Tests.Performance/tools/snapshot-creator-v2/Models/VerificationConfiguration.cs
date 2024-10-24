using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record VerificationConfiguration
{
    [JsonPropertyName(nameof(App))] public AppVerificationConfiguration App { get; set; } = null!;

    [JsonPropertyName(nameof(Connector))] public ConnectorVerificationConfiguration Connector { get; set; } = null!;
}

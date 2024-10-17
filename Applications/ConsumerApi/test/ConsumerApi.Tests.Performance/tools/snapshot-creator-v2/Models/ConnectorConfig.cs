using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models.Base;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record ConnectorConfig : BaseConfig
{
    [JsonPropertyName(nameof(TotalNumberOfAvailableRelationships))]
    public int TotalNumberOfAvailableRelationships { get; set; }
}

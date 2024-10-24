using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models.Base;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record ConnectorVerificationConfiguration : BaseVerificationConfiguration
{
    [JsonPropertyName(nameof(TotalNumberOfAvailableRelationships))]
    public long TotalNumberOfAvailableRelationships { get; set; }
}

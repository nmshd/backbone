using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models.Base;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record AppConfig : BaseConfig
{
    [JsonPropertyName(nameof(TotalNumberOfRelationships))]
    public int TotalNumberOfRelationships { get; set; }
}

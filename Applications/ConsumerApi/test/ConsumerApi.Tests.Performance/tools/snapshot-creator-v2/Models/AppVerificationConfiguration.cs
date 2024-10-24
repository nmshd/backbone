using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models.Base;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record AppVerificationConfiguration : BaseVerificationConfiguration
{
    [JsonPropertyName(nameof(TotalNumberOfRelationships))]
    public long TotalNumberOfRelationships { get; set; }
}

using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record VerificationConfiguration
{
    [JsonPropertyName(nameof(TotalNumberOfRelationships))]
    public int TotalNumberOfRelationships { get; set; }


    [JsonPropertyName(nameof(TotalConnectorSentMessages))]
    public long TotalConnectorSentMessages { get; set; }


    [JsonPropertyName(nameof(TotalAppSentMessages))]
    public long TotalAppSentMessages { get; set; }
}

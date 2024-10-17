using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models.Base;

public record BaseConfig
{
    [JsonPropertyName(nameof(TotalNumberOfSentMessages))]
    public long TotalNumberOfSentMessages { get; set; }

    [JsonPropertyName(nameof(TotalNumberOfReceivedMessages))]
    public long TotalNumberOfReceivedMessages { get; set; }

    [JsonPropertyName(nameof(NumberOfReceivedMessagesAddOn))]
    public long NumberOfReceivedMessagesAddOn { get; set; }
}

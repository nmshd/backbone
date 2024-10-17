using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models.Base;

public record BaseConfig
{
    [JsonPropertyName(nameof(TotalNumberOfSentMessages))]
    public int TotalNumberOfSentMessages { get; set; }

    [JsonPropertyName(nameof(TotalNumberOfReceivedMessages))]
    public int TotalNumberOfReceivedMessages { get; set; }

    [JsonPropertyName(nameof(NumberOfReceivedMessagesAddOn))]
    public int NumberOfReceivedMessagesAddOn { get; set; }
}

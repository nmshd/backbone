using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record PoolConfiguration
{
    [JsonPropertyName(nameof(Type))] public string Type { get; set; } = null!;

    [JsonPropertyName(nameof(Name))] public string Name { get; set; } = null!;

    [JsonPropertyName(nameof(Alias))] public string Alias { get; set; } = null!;

    [JsonPropertyName(nameof(Amount))] public long Amount { get; set; }

    [JsonPropertyName(nameof(NumberOfRelationshipTemplates))]
    public int NumberOfRelationshipTemplates { get; set; }

    [JsonPropertyName(nameof(NumberOfRelationships))]
    public int NumberOfRelationships { get; set; }

    [JsonPropertyName(nameof(NumberOfSentMessages))]
    public int NumberOfSentMessages { get; set; }

    [JsonPropertyName(nameof(NumberOfReceivedMessages))]
    public int NumberOfReceivedMessages { get; set; }

    [JsonPropertyName(nameof(NumberOfDatawalletModifications))]
    public int NumberOfDatawalletModifications { get; set; }

    [JsonPropertyName(nameof(NumberOfDevices))]
    public int NumberOfDevices { get; set; }

    [JsonPropertyName(nameof(NumberOfChallenges))]
    public int NumberOfChallenges { get; set; }
}

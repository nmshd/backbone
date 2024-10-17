using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record IdentityPoolConfiguration
{
    [JsonPropertyName(nameof(Type))]
    public string Type { get; set; } = null!;

    [JsonPropertyName(nameof(Name))]
    public string Name { get; set; } = null!;

    [JsonPropertyName(nameof(Alias))]
    public string Alias { get; set; } = null!;

    [JsonPropertyName(nameof(Amount))]
    public long Amount { get; set; }

    [JsonPropertyName(nameof(NumberOfRelationshipTemplates))]
    public long NumberOfRelationshipTemplates { get; set; }

    [JsonPropertyName(nameof(NumberOfRelationships))]
    public long NumberOfRelationships { get; set; }

    [JsonPropertyName(nameof(NumberOfSentMessages))]
    public long NumberOfSentMessages { get; set; }

    [JsonPropertyName(nameof(NumberOfReceivedMessages))]
    public long NumberOfReceivedMessages { get; set; }

    [JsonPropertyName(nameof(NumberOfDatawalletModifications))]
    public long NumberOfDatawalletModifications { get; set; }

    [JsonPropertyName(nameof(NumberOfDevices))]
    public long NumberOfDevices { get; set; }

    [JsonPropertyName(nameof(NumberOfChallenges))]
    public long NumberOfChallenges { get; set; }
}

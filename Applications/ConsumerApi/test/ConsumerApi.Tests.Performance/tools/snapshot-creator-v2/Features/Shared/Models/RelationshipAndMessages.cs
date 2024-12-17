using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record RelationshipAndMessages(
    [property: JsonPropertyName("SenderPool")]
    string SenderPoolAlias,
    [property: JsonPropertyName("SenderIdentityAddress")]
    int SenderIdentityAddress,
    [property: JsonPropertyName("RecipientPool")]
    string RecipientPoolAlias,
    [property: JsonPropertyName("RecipientIdentityAddress")]
    int RecipientIdentityAddress)
{
    [JsonPropertyName("NumberOfSentMessages")]
    public long NumberOfSentMessages { get; set; }

    [Ignore, JsonIgnore]
    public IdentityPoolType SenderIdentityPoolType => SenderPoolAlias.FirstOrDefault() switch
    {
        'n' => IdentityPoolType.Never,
        'a' => IdentityPoolType.App,
        'c' => IdentityPoolType.Connector,
        _ => throw new InvalidOperationException(POOL_TYPE_UNKNOWN)
    };

    [Ignore, JsonIgnore]
    public IdentityPoolType RecipientIdentityPoolType => RecipientPoolAlias.FirstOrDefault() switch
    {
        'n' => IdentityPoolType.Never,
        'a' => IdentityPoolType.App,
        'c' => IdentityPoolType.Connector,
        _ => throw new InvalidOperationException(POOL_TYPE_UNKNOWN)
    };
}

using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record RelationshipAndMessages(
    string SenderPool,
    int SenderIdentityAddress,
    string RecipientPool,
    int RecipientIdentityAddress)
{
    public long NumberOfSentMessages { get; set; }

    [Ignore, JsonIgnore]
    public IdentityPoolType RecipientIdentityPoolType => RecipientPool.FirstOrDefault() switch
    {
        'n' => IdentityPoolType.Never,
        'a' => IdentityPoolType.App,
        'c' => IdentityPoolType.Connector,
        _ => throw new InvalidOperationException(POOL_TYPE_UNKNOWN)
    };
}

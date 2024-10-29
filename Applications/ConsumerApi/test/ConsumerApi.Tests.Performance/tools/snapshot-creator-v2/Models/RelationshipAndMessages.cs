using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record RelationshipAndMessages(
    string SenderPool,
    int SenderIdentityAddress,
    string ReceiverPool,
    int ReceiverIdentityAddress)
{
    public long NumberOfSentMessages { get; set; }

    [Ignore, JsonIgnore]
    public IdentityPoolType ReceiverIdentityPoolType => ReceiverPool.FirstOrDefault() switch
    {
        'n' => IdentityPoolType.Never,
        'a' => IdentityPoolType.App,
        'c' => IdentityPoolType.Connector,
        _ => throw new InvalidOperationException(POOL_TYPE_UNKNOWN)
    };
}

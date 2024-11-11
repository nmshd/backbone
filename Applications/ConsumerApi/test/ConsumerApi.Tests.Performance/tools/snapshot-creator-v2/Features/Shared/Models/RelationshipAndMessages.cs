using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record RelationshipAndMessages(
    string SenderPoolAlias,
    int SenderIdentityAddress,
    string RecipientPoolAlias,
    int RecipientIdentityAddress)
{
    public long NumberOfSentMessages { get; set; }

    [Ignore, JsonIgnore]
    public IdentityPoolType RecipientIdentityPoolType => RecipientPoolAlias.FirstOrDefault() switch
    {
        'n' => IdentityPoolType.Never,
        'a' => IdentityPoolType.App,
        'c' => IdentityPoolType.Connector,
        _ => throw new InvalidOperationException(POOL_TYPE_UNKNOWN)
    };
}

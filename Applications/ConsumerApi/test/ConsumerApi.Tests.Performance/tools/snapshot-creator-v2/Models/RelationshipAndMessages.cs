using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record RelationshipAndMessages(
    string SenderPool,
    int SenderIdentityAddress,
    string ReceiverPool,
    int ReceiverIdentityAddress,
    [property: Ignore, JsonIgnore] IdentityPoolType ReceiverIdentityPoolType)
{
    public int NumberOfSentMessages { get; set; }
}

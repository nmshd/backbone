using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record RelationshipAndMessages(
    string SenderPool,
    int SenderIdentityId,
    string ReceiverPool,
    int ReceiverIdentityId)
{
    public int NumberOfSentMessages { get; set; }

    [Ignore] public bool ModuloSentApplied { get; set; }
    [Ignore] public bool ModuloReceivedApplied { get; set; }
}

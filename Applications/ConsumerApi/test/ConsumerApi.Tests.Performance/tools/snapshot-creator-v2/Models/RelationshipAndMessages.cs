namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record RelationshipAndMessages(
    string SenderPool,
    int SenderIdentityId,
    string ReceiverPool,
    int ReceiverIdentityId,
    int NumberOfSentMessages);

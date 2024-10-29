using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;

public interface IRelationshipAndMessagesExcelWriter
{
    Task Write(RelationshipAndMessages[] relationshipAndMessages, string worksheet, string filePath);
}

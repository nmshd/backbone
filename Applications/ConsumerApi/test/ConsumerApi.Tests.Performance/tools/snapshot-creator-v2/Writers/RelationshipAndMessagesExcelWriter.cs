using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Writers;

public class RelationshipAndMessagesExcelWriter : IRelationshipAndMessagesExcelWriter
{
    public async Task Write(RelationshipAndMessages[] relationshipAndMessages, string worksheet, string filePath)
    {
        await new ExcelMapper().SaveAsync(filePath, relationshipAndMessages, worksheet);
    }
}

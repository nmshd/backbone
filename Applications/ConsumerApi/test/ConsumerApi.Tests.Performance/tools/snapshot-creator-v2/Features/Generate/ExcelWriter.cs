using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public class ExcelWriter : IExcelWriter
{
    private static async Task Write<T>(string filePath, IEnumerable<T> data)
    {
        var excelMapper = new ExcelMapper();
        await excelMapper.SaveAsync(filePath, data);
    }

    public async Task WritePoolConfigurations(string snapshotFolder, string worksheet, List<PoolConfiguration> poolConfigurations)
    {
        var excelFilePath = Path.Combine(snapshotFolder, $"pool-config.{worksheet}.xlsx");
        await Write(excelFilePath, poolConfigurations);
    }

    public async Task WriteRelationshipsAndMessages(string snapshotFolder, string worksheet, List<RelationshipAndMessages> relationshipAndMessages)
    {
        var excelFilePath = Path.Combine(snapshotFolder, $"relationships.{worksheet}.xlsx");
        await Write(excelFilePath, relationshipAndMessages);
    }
}

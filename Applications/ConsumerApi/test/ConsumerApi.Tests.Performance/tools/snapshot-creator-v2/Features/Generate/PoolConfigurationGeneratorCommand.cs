using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public class PoolConfigurationGeneratorCommand(
    IPerformanceTestConfigurationExcelReader performanceTestConfigurationExcelReader,
    IRelationshipAndMessagesGenerator relationshipAndMessagesGenerator,
    IPoolConfigurationJsonWriter poolConfigurationJsonWriter)
    : ICommand<PoolConfigurationGeneratorCommandArgs, StatusMessage>
{
    public async Task<StatusMessage> Execute(PoolConfigurationGeneratorCommandArgs parameter)
    {
        StatusMessage result;
        try
        {
            var poolConfigFromExcel = await performanceTestConfigurationExcelReader.Read(parameter.ExcelFilePath, parameter.WorkSheetName);

            var relationshipAndMessages = relationshipAndMessagesGenerator.Generate(poolConfigFromExcel);

            poolConfigFromExcel.RelationshipAndMessages.Clear();
            poolConfigFromExcel.RelationshipAndMessages.AddRange(relationshipAndMessages);

            var path = Path.GetDirectoryName(parameter.ExcelFilePath);

            var snapshotFolder = Path.Combine(path!, $"{DateTime.Now:yyyyMMddHHmmss}-snapshot-{parameter.WorkSheetName}");

            if (Directory.Exists(snapshotFolder))
            {
                Directory.Delete(snapshotFolder, true);
            }

            Directory.CreateDirectory(snapshotFolder);

            var poolConfigJsonFilePath = Path.Combine(snapshotFolder!, $"{POOL_CONFIG_JSON_WITH_RELATIONSHIP_AND_MESSAGES}.{parameter.WorkSheetName}.{JSON_FILE_EXT}");
            result = await poolConfigurationJsonWriter.Write(poolConfigFromExcel, poolConfigJsonFilePath);
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message);
        }

        return result;
    }
}

using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands.Args;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands;

public class PoolConfigurationJsonGeneratorCommand(
    IPoolConfigurationJsonWriter poolConfigurationJsonWriter,
    IPerformanceTestConfigurationExcelReader performanceTestConfigurationExcelReader)
    : ICommand<PoolConfigurationJsonGeneratorCommandArgs, StatusMessage>
{
    public async Task<StatusMessage> Execute(PoolConfigurationJsonGeneratorCommandArgs parameter)
    {
        StatusMessage result;
        try
        {
            var poolConfigFromExcel = await performanceTestConfigurationExcelReader.Read(parameter.ExcelFilePath, parameter.WorkSheetName);

            var path = Path.GetDirectoryName(parameter.ExcelFilePath);
            var poolConfigJsonFilePath = Path.Combine(path!, $"{POOL_CONFIG_JSON_NAME}.{parameter.WorkSheetName}.{JSON_FILE_EXT}");

            result = await poolConfigurationJsonWriter.Write(poolConfigFromExcel, poolConfigJsonFilePath);
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message);
        }

        return result;
    }
}

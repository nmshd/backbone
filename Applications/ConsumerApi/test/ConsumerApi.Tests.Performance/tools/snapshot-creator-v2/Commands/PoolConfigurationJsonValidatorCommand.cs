using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands.Args;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands;

public class PoolConfigurationJsonValidatorCommand(
    IPerformanceTestConfigurationExcelReader performanceTestConfigurationExcelReader,
    IPerformanceTestConfigurationJsonReader performanceTestConfigurationJsonReader,
    IRelationshipAndMessagesGenerator relationshipAndMessagesGenerator,
    IPoolConfigurationJsonValidator poolConfigurationJsonValidator) : ICommand<PoolConfigurationJsonValidatorCommandArgs, bool>
{
    public async Task<bool> Execute(PoolConfigurationJsonValidatorCommandArgs parameter)
    {
        var poolConfigFromExcel = await performanceTestConfigurationExcelReader.Read(parameter.ExcelFilePath, parameter.WorkSheetName);

        var relationshipAndMessages = relationshipAndMessagesGenerator.Generate(poolConfigFromExcel);
        poolConfigFromExcel.RelationshipAndMessages.Clear();
        poolConfigFromExcel.RelationshipAndMessages.AddRange(relationshipAndMessages);

        var poolConfigFromJson = await performanceTestConfigurationJsonReader.Read(parameter.JsonFilePath);

        var result = await poolConfigurationJsonValidator.Validate(poolConfigFromJson, poolConfigFromExcel);

        return result;
    }
}

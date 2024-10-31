using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;

public class PoolConfigurationValidatorCommand(
    IPerformanceTestConfigurationExcelReader performanceTestConfigurationExcelReader,
    IPerformanceTestConfigurationJsonReader performanceTestConfigurationJsonReader,
    IRelationshipAndMessagesGenerator relationshipAndMessagesGenerator,
    IPoolConfigurationJsonValidator poolConfigurationJsonValidator) : ICommand<PoolConfigurationValidatorCommandArgs, bool>
{
    public async Task<bool> Execute(PoolConfigurationValidatorCommandArgs parameter)
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

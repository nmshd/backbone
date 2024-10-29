using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands.Args;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands;

public class RelationshipAndMessagesGeneratorCommand(
    IPerformanceTestConfigurationJsonReader performanceTestConfigurationJsonReader,
    IRelationshipAndMessagesGenerator relationshipAndMessagesGenerator,
    IRelationshipAndMessagesExcelWriter relationshipAndMessagesExcelWriter) : ICommand<RelationshipAndMessagesGeneratorCommandArgs, StatusMessage>
{
    public async Task<StatusMessage> Execute(RelationshipAndMessagesGeneratorCommandArgs parameter)
    {
        var excelFilePath = GetExcelSaveFilePath(parameter);
        try
        {
            var poolConfig = await performanceTestConfigurationJsonReader.Read(parameter.JsonFilePath);
            var relationshipAndMessages = relationshipAndMessagesGenerator.Generate(poolConfig);
            await relationshipAndMessagesExcelWriter.Write(relationshipAndMessages, parameter.WorkSheetName, excelFilePath);
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message);
        }

        return new StatusMessage(true, Path.GetFullPath(excelFilePath));
    }

    private static string GetExcelSaveFilePath(RelationshipAndMessagesGeneratorCommandArgs parameter) =>
        Path.Combine(
            Path.GetDirectoryName(parameter.JsonFilePath) ?? throw new InvalidOperationException(),
            $"{RELATIONSHIPS_AND_MESSAGE_POOL_CONFIGS_FILE_NAME}.{parameter.WorkSheetName}.{EXCEL_FILE_EXT}");
}

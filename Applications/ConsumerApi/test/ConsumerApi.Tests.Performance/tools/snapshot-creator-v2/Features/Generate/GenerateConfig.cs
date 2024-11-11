using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public record GenerateConfig
{
    public record Command(string ExcelFilePath, string WorkSheetName) : IRequest<StatusMessage>;

    public class CommandHandler(
        IPerformanceTestConfigurationExcelReader performanceTestConfigurationExcelReader,
        IRelationshipAndMessagesGenerator relationshipAndMessagesGenerator,
        IPoolConfigurationJsonWriter poolConfigurationJsonWriter)
        : IRequestHandler<Command, StatusMessage>
    {
        public async Task<StatusMessage> Handle(Command request, CancellationToken cancellationToken)
        {
            StatusMessage result;
            try
            {
                var poolConfigFromExcel = await performanceTestConfigurationExcelReader.Read(request.ExcelFilePath, request.WorkSheetName);

                var relationshipAndMessages = relationshipAndMessagesGenerator.Generate(poolConfigFromExcel);

                poolConfigFromExcel.RelationshipAndMessages.Clear();
                poolConfigFromExcel.RelationshipAndMessages.AddRange(relationshipAndMessages);

                var path = Path.GetDirectoryName(request.ExcelFilePath);

                var snapshotFolder = Path.Combine(path!, $"{DateTime.Now:yyyyMMddHHmmss}-snapshot-{request.WorkSheetName}");

                if (Directory.Exists(snapshotFolder))
                {
                    Directory.Delete(snapshotFolder, true);
                }

                Directory.CreateDirectory(snapshotFolder);

                var poolConfigJsonFilePath = Path.Combine(snapshotFolder!, $"{POOL_CONFIG_JSON_WITH_RELATIONSHIP_AND_MESSAGES}.{request.WorkSheetName}.{JSON_FILE_EXT}");
                result = await poolConfigurationJsonWriter.Write(poolConfigFromExcel, poolConfigJsonFilePath);
            }
            catch (Exception e)
            {
                return new StatusMessage(false, e.Message);
            }

            return result;
        }
    }
}

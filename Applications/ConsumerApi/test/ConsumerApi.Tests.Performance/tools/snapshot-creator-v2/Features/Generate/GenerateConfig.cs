using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public abstract record GenerateConfig
{
    public record Command(string ExcelFilePath, string WorkSheetName, bool DebugMode = false) : IRequest<StatusMessage>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public class CommandHandler(
        IPoolConfigurationExcelReader poolConfigurationExcelReader,
        IRelationshipAndMessagesGenerator relationshipAndMessagesGenerator,
        IPoolConfigurationJsonWriter poolConfigurationJsonWriter,
        IExcelWriter excelWriter)
        : IRequestHandler<Command, StatusMessage>
    {
        public async Task<StatusMessage> Handle(Command request, CancellationToken cancellationToken)
        {
            StatusMessage result;
            try
            {
                var poolConfigFromExcel = await poolConfigurationExcelReader.Read(request.ExcelFilePath, request.WorkSheetName);

                var relationshipAndMessages = relationshipAndMessagesGenerator.Generate(poolConfigFromExcel);

                poolConfigFromExcel.RelationshipAndMessages.Clear();
                poolConfigFromExcel.RelationshipAndMessages.AddRange(relationshipAndMessages);

                var path = Path.GetDirectoryName(request.ExcelFilePath);

                var snapshotFolder = Path.Combine(path!, $"PoolConfig-{request.WorkSheetName.ToUpper()}.{DateTime.UtcNow:yyyyMMdd-HHmmss}");

                if (Directory.Exists(snapshotFolder))
                {
                    Directory.Delete(snapshotFolder, true);
                }

                Directory.CreateDirectory(snapshotFolder);

                var poolConfigJsonFilePath = Path.Combine(snapshotFolder, $"pool-config.{request.WorkSheetName}.json");
                result = await poolConfigurationJsonWriter.Write(poolConfigFromExcel, poolConfigJsonFilePath);

                if (!request.DebugMode)
                {
                    return result;
                }

                var excelFilePath = Path.Combine(snapshotFolder, $"pool-config.{request.WorkSheetName}.xlsx");
                await excelWriter.Write(excelFilePath, poolConfigFromExcel.PoolConfigurations);

                excelFilePath = Path.Combine(snapshotFolder, $"relationships.{request.WorkSheetName}.xlsx");
                await excelWriter.Write(excelFilePath, poolConfigFromExcel.RelationshipAndMessages);
            }
            catch (Exception e)
            {
                return new StatusMessage(false, e.Message, e);
            }

            return result;
        }
    }
}

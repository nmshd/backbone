using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public abstract record GenerateConfig
{
    public record Command(string ExcelFilePath, string WorkSheetName, bool DebugMode = false) : IRequest<GenerateConfigStatusMessage>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public class CommandHandler(
        IPoolConfigurationExcelReader poolConfigurationExcelReader,
        IRelationshipAndMessagesGenerator relationshipAndMessagesGenerator,
        IPoolConfigurationJsonWriter poolConfigurationJsonWriter,
        IExcelWriter excelWriter)
        : IRequestHandler<Command, GenerateConfigStatusMessage>
    {
        public async Task<GenerateConfigStatusMessage> Handle(Command request, CancellationToken cancellationToken)
        {
            GenerateConfigStatusMessage result;
            try
            {
                var poolConfigFromExcel = await poolConfigurationExcelReader.Read(request.ExcelFilePath, request.WorkSheetName);

                var relationshipAndMessages = relationshipAndMessagesGenerator.Generate(poolConfigFromExcel);
                poolConfigFromExcel.RelationshipAndMessages.Clear();
                poolConfigFromExcel.RelationshipAndMessages.AddRange(relationshipAndMessages);

                var path = Path.GetDirectoryName(request.ExcelFilePath);
                var poolConfigurationFolder = Path.Combine(path!, $"PoolConfig-{request.WorkSheetName.ToUpper()}.{DateTime.UtcNow:yyyyMMdd-HHmmss}");
                Directory.CreateDirectory(poolConfigurationFolder);

                var poolConfigJsonFilePath = Path.Combine(poolConfigurationFolder, $"pool-config.{request.WorkSheetName}.json");
                var poolConfigurationResult = await poolConfigurationJsonWriter.Write(poolConfigFromExcel, poolConfigJsonFilePath);

                if (!poolConfigurationResult.Status)
                {
                    throw new InvalidOperationException(POOL_CONFIG_FILE_WRITE_ERROR, innerException: poolConfigurationResult.Exception);
                }

                result = new GenerateConfigStatusMessage(poolConfigurationResult.Status, poolConfigurationFolder, poolConfigurationResult.Message);

                if (!request.DebugMode)
                {
                    return result;
                }

                await excelWriter.WritePoolConfigurations(poolConfigurationFolder, request.WorkSheetName, poolConfigFromExcel.PoolConfigurations);
                await excelWriter.WriteRelationshipsAndMessages(poolConfigurationFolder, request.WorkSheetName, poolConfigFromExcel.RelationshipAndMessages);
            }
            catch (Exception e)
            {
                return new GenerateConfigStatusMessage(false, null, e.Message, e);
            }

            return result;
        }
    }
}

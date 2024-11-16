using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;

public record VerifyConfig
{
    public record Command(string ExcelFilePath, string WorkSheetName, string JsonFilePath) : IRequest<bool>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(
        IPoolConfigurationExcelReader PoolConfigurationExcelReader,
        IPoolConfigurationJsonReader PoolConfigurationJsonReader,
        IRelationshipAndMessagesGenerator RelationshipAndMessagesGenerator,
        IPoolConfigurationJsonValidator PoolConfigurationJsonValidator,
#if DEBUG
        IExcelWriter ExcelWriter
#endif
    ) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var poolConfigFromExcel = await PoolConfigurationExcelReader.Read(request.ExcelFilePath, request.WorkSheetName);

            var relationshipAndMessages = RelationshipAndMessagesGenerator.Generate(poolConfigFromExcel);

            poolConfigFromExcel.RelationshipAndMessages.Clear();
            poolConfigFromExcel.RelationshipAndMessages.AddRange(relationshipAndMessages);

            var poolConfigFromJson = await PoolConfigurationJsonReader.Read(request.JsonFilePath);

            var result = await PoolConfigurationJsonValidator.Validate(poolConfigFromJson, poolConfigFromExcel);

#if DEBUG
            var path = Path.GetDirectoryName(request.ExcelFilePath);
            var jsonFilePath = Path.Combine(path!, $"excel-config.relationships.{request.WorkSheetName}.xlsx");
            await ExcelWriter.Write(jsonFilePath, poolConfigFromExcel.RelationshipAndMessages);

            jsonFilePath = Path.Combine(path!, $"pool-config.relationships.{request.WorkSheetName}.xlsx");
            await ExcelWriter.Write(jsonFilePath, poolConfigFromJson.RelationshipAndMessages);
#endif

            return result;
        }
    }
}

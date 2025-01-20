using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;

public abstract class VerifyConfig
{
    public record Command(string ExcelFilePath, string WorkSheetName, string JsonFilePath) : IRequest<bool>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public class CommandHandler(
        IPoolConfigurationExcelReader poolConfigurationExcelReader,
        IPoolConfigurationJsonReader poolConfigurationJsonReader,
        IRelationshipAndMessagesGenerator relationshipAndMessagesGenerator,
        IPoolConfigurationJsonValidator poolConfigurationJsonValidator) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var poolConfigFromExcel = await poolConfigurationExcelReader.Read(request.ExcelFilePath, request.WorkSheetName);

            var relationshipAndMessages = relationshipAndMessagesGenerator.Generate(poolConfigFromExcel);

            poolConfigFromExcel.RelationshipAndMessages.Clear();
            poolConfigFromExcel.RelationshipAndMessages.AddRange(relationshipAndMessages);

            var poolConfigFromJson = await poolConfigurationJsonReader.Read(request.JsonFilePath);

            if (poolConfigFromJson is null) return false;

            var result = await poolConfigurationJsonValidator.Validate(poolConfigFromJson, poolConfigFromExcel);

            return result;
        }
    }
}

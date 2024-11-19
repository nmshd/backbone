using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;

public record CreateSnapshot
{
    public record Command(
        string BaseAddress,
        string ClientId,
        string ClientSecret,
        string JsonFilePath) : IRequest<StatusMessage>;

    public record CommandHandler(
        ILogger<CommandHandler> Logger,
        IPoolConfigurationJsonReader PoolConfigurationJsonReader,
        IMediator Mediator,
        IOutputHelper OutputHelper)
        : IRequestHandler<Command, StatusMessage>
    {
        public async Task<StatusMessage> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation("Creating pool configuration with relationships and messages ...");

                var outputDirName = CreateSnapshotDirAndCopyPoolConfigFiles(request.JsonFilePath);

                var poolConfig = await PoolConfigurationJsonReader.Read(request.JsonFilePath);

                if (poolConfig is null)
                {
                    return new StatusMessage(false, "Pool configuration could not be read.");
                }

                var clientCredentials = new ClientCredentials(request.ClientId, request.ClientSecret);

                var identities = await Mediator.Send(new CreateIdentities.Command(poolConfig.IdentityPoolConfigurations, request.BaseAddress, clientCredentials), cancellationToken);
                Logger.LogInformation("Identities created");

                await Mediator.Send(new CreateDevices.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteIdentities(outputDirName, identities);
                Logger.LogInformation("Devices added");

                await Mediator.Send(new CreateRelationshipTemplates.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteRelationshipTemplates(outputDirName, identities);
                Logger.LogInformation("Relationship templates created");

                await Mediator.Send(new CreateRelationships.Command(identities, poolConfig.RelationshipAndMessages, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteRelationships(outputDirName, identities);
                Logger.LogInformation("Relationships created");

                await Mediator.Send(new CreateChallenges.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteChallenges(outputDirName, identities);
                Logger.LogInformation("Challenges created");

                await Mediator.Send(new CreateMessages.Command(identities, poolConfig.RelationshipAndMessages, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteMessages(outputDirName, identities);
                Logger.LogInformation("Messages created");

                await Mediator.Send(new CreateDatawalletModifications.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteDatawalletModifications(outputDirName, identities);
                Logger.LogInformation("DatawalletModifications created");
            }
            catch (Exception e)
            {
                return new StatusMessage(false, e.Message);
            }


            return new StatusMessage(true, "Pool configuration with relationships and messages created successfully.");
        }

        private static string CreateSnapshotDirAndCopyPoolConfigFiles(string jsonFullFilePath)
        {
            if (!File.Exists(jsonFullFilePath))
            {
                throw new FileNotFoundException("Pool configuration file not found.", jsonFullFilePath);
            }

            var loadTestToken = Path.GetFileNameWithoutExtension(jsonFullFilePath).Split('.').Last();
            var workloadName = loadTestToken.ToUpper();
            var snapshotDirectoryName = Path.Combine(AppContext.BaseDirectory, $"Snapshot-{workloadName}.{DateTime.UtcNow:yyyyMMdd-HHmmss}");

            Directory.CreateDirectory(snapshotDirectoryName);

            var configDirectory = Path.Combine(snapshotDirectoryName, "PoolConfig");
            Directory.CreateDirectory(configDirectory);

            var poolConfigDirectoryName = Path.GetDirectoryName(jsonFullFilePath)!;
            var poolConfigs = Directory.GetFiles(poolConfigDirectoryName);

            foreach (var sourceFileName in poolConfigs)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFileName);
                if (!fileNameWithoutExtension.EndsWith(loadTestToken)) continue;

                var fileName = Path.GetFileName(sourceFileName);
                var destFileName = Path.Combine(configDirectory, fileName);
                File.Copy(sourceFileName, destFileName, true);
            }

            return snapshotDirectoryName;
        }
    }
}

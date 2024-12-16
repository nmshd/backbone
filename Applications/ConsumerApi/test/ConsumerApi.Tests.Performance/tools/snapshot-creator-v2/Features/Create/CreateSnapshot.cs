using System.Diagnostics;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;

public abstract record CreateSnapshot
{
    public record Command(
        string BaseAddress,
        string ClientId,
        string ClientSecret,
        string JsonFilePath,
        bool ClearDatabase,
        bool BackupDatabase,
        bool ClearOnly) : IRequest<StatusMessage>;

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        IPoolConfigurationJsonReader poolConfigurationJsonReader,
        IMediator mediator,
        IOutputHelper outputHelper,
        IDatabaseRestoreHelper databaseRestoreHelper)
        : IRequestHandler<Command, StatusMessage>
    {
        internal string? OutputDirName { get; private set; }

        public async Task<StatusMessage> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.ClearOnly)
                {
                    var result = await databaseRestoreHelper.RestoreCleanDatabase();

                    if (!result.Status)
                    {
                        return result;
                    }

                    logger.LogInformation("Restore clean-db completed: {Message}", result.Message);
                    return new StatusMessage(true, CLEAN_DB_SUCCEED_MESSAGE);
                }

                logger.LogInformation("Creating pool configuration with relationships and messages ...");

                if (request.ClearDatabase)
                {
                    var result = await databaseRestoreHelper.RestoreCleanDatabase();

                    if (!result.Status)
                    {
                        return result;
                    }

                    logger.LogInformation("Restore clean-db completed: {Message}", result.Message);
                }


                OutputDirName = CreateSnapshotDirAndCopyPoolConfigFiles(request.JsonFilePath);

                var poolConfig = await poolConfigurationJsonReader.Read(request.JsonFilePath);

                if (poolConfig is null)
                {
                    return new StatusMessage(false, POOL_CONFIG_FILE_READ_ERROR);
                }

                var clientCredentials = new ClientCredentials(request.ClientId, request.ClientSecret);

                Stopwatch stopwatch = new();
                stopwatch.Start();

                var identities = await mediator.Send(new CreateIdentities.Command(poolConfig.IdentityPoolConfigurations, request.BaseAddress, clientCredentials), cancellationToken);

                stopwatch.Stop();
                var totalRunTime = stopwatch.Elapsed;
                logger.LogInformation("Identities created in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await mediator.Send(new CreateDevices.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                await outputHelper.WriteIdentities(OutputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                logger.LogInformation("Devices added in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await mediator.Send(new CreateChallenges.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                await outputHelper.WriteChallenges(OutputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                logger.LogInformation("Challenges created in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await mediator.Send(new CreateDatawalletModifications.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                await outputHelper.WriteDatawalletModifications(OutputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                logger.LogInformation("DatawalletModifications created in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await mediator.Send(new CreateRelationshipTemplates.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                await outputHelper.WriteRelationshipTemplates(OutputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                logger.LogInformation("Relationship templates created in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await mediator.Send(new CreateRelationships.Command(identities, poolConfig.RelationshipAndMessages, request.BaseAddress, clientCredentials), cancellationToken);
                await outputHelper.WriteRelationships(OutputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                logger.LogInformation("Relationships created {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await mediator.Send(new CreateMessages.Command(identities, poolConfig.RelationshipAndMessages, request.BaseAddress, clientCredentials), cancellationToken);
                await outputHelper.WriteMessages(OutputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                logger.LogInformation("Messages created in {ElapsedTime}", stopwatch.Elapsed);

                logger.LogInformation("Pool configuration with relationships and messages created in {ElapsedTime}", totalRunTime);

                if (request.BackupDatabase)
                {
                    var result = await databaseRestoreHelper.BackupDatabase(OutputDirName);

                    if (!result.Status)
                    {
                        return result;
                    }

                    logger.LogInformation("Backup completed: {Message}", result.Message);
                }
            }
            catch (Exception e)
            {
                return new StatusMessage(false, e.Message, e);
            }

            return new StatusMessage(true, SNAPSHOT_CREATION_SUCCEED_MESSAGE);
        }

        private static string CreateSnapshotDirAndCopyPoolConfigFiles(string jsonFullFilePath)
        {
            if (!File.Exists(jsonFullFilePath))
            {
                throw new FileNotFoundException(POOL_CONFIG_FILE_NOT_FOUND_ERROR, jsonFullFilePath);
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

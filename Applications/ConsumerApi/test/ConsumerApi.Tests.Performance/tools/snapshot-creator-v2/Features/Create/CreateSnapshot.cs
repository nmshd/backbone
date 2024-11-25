using System.Diagnostics;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;

public abstract record CreateSnapshot
{
    public enum PerformanceLoadTest
    {
        Low,
        Medium,
        High
    }

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
        private static PerformanceLoadTest GetLoadTag(string jsonFullFilePath)
        {
            if (!File.Exists(jsonFullFilePath))
            {
                throw new FileNotFoundException("Pool configuration file not found.", jsonFullFilePath);
            }

            var loadTestToken = Path.GetFileNameWithoutExtension(jsonFullFilePath).Split('.').Last();

            return loadTestToken switch
            {
                "low" => PerformanceLoadTest.Low,
                "medium" => PerformanceLoadTest.Medium,
                "heavy" => PerformanceLoadTest.High,
                _ => PerformanceLoadTest.Medium
            };
        }

        public async Task<StatusMessage> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation("Creating pool configuration with relationships and messages ...");


                var loadTag = GetLoadTag(request.JsonFilePath);
                var outputDirName = CreateSnapshotDirAndCopyPoolConfigFiles(request.JsonFilePath);
                var poolConfig = await PoolConfigurationJsonReader.Read(request.JsonFilePath);

                if (poolConfig is null)
                {
                    return new StatusMessage(false, "Pool configuration could not be read.");
                }

                var clientCredentials = new ClientCredentials(request.ClientId, request.ClientSecret);

                Stopwatch stopwatch = new();
                stopwatch.Start();

                var identities = await Mediator.Send(new CreateIdentities.Command(loadTag, poolConfig.IdentityPoolConfigurations, request.BaseAddress, clientCredentials), cancellationToken);

                stopwatch.Stop();
                var totalRunTime = stopwatch.Elapsed;
                Logger.LogInformation("Identities created in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await Mediator.Send(new CreateDevices.Command(loadTag, identities, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteIdentities(outputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                Logger.LogInformation("Devices added in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await Mediator.Send(new CreateChallenges.Command(loadTag, identities, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteChallenges(outputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                Logger.LogInformation("Challenges created in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await Mediator.Send(new CreateDatawalletModifications.Command(loadTag, identities, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteDatawalletModifications(outputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                Logger.LogInformation("DatawalletModifications created in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await Mediator.Send(new CreateRelationshipTemplates.Command(loadTag, identities, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteRelationshipTemplates(outputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                Logger.LogInformation("Relationship templates created in {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await Mediator.Send(new CreateRelationships.Command(loadTag, identities, poolConfig.RelationshipAndMessages, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteRelationships(outputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                Logger.LogInformation("Relationships created {ElapsedTime}", stopwatch.Elapsed);

                stopwatch.Restart();

                await Mediator.Send(new CreateMessages.Command(loadTag, identities, poolConfig.RelationshipAndMessages, request.BaseAddress, clientCredentials), cancellationToken);
                await OutputHelper.WriteMessages(outputDirName, identities);

                stopwatch.Stop();
                totalRunTime += stopwatch.Elapsed;
                Logger.LogInformation("Messages created in {ElapsedTime}", stopwatch.Elapsed);

                Logger.LogInformation("Pool configuration with relationships and messages created in {ElapsedTime}", totalRunTime);
            }
            catch (Exception e)
            {
                return new StatusMessage(false, e.Message, e);
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

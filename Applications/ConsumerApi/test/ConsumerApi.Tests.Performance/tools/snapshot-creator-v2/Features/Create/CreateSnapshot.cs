using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling;
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
        private readonly string _outputDirName = Path.Combine(AppContext.BaseDirectory, $"Snapshot.{SystemTime.UtcNow:yyyyMMdd-HHmmss}");

        public async Task<StatusMessage> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation("Creating pool configuration with relationships and messages ...");

                var poolConfig = await PoolConfigurationJsonReader.Read(request.JsonFilePath);
                var clientCredentials = new ClientCredentials(request.ClientId, request.ClientSecret);

                var identities = await Mediator.Send(new CreateIdentities.Command(poolConfig.IdentityPoolConfigurations, request.BaseAddress, clientCredentials), cancellationToken);

                OutputHelper.WriteIdentities(_outputDirName, identities);
                Logger.LogInformation("Identities created");

                await Mediator.Send(new AddDevices.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                Logger.LogInformation("Devices added");

                await Mediator.Send(new CreateRelationshipTemplates.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);

                OutputHelper.WriteRelationshipTemplates(_outputDirName, identities);
                Logger.LogInformation("Relationship templates created");

                await Mediator.Send(new CreateRelationships.Command(identities, poolConfig.RelationshipAndMessages, request.BaseAddress, clientCredentials), cancellationToken);

                OutputHelper.WriteRelationships(_outputDirName, identities);
                Logger.LogInformation("Relationships created");

                await Mediator.Send(new CreateChallenges.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);

                OutputHelper.WriteChallenges(_outputDirName, identities);
                Logger.LogInformation("Challenges created");

                await Mediator.Send(new CreateMessages.Command(identities, poolConfig.RelationshipAndMessages, request.BaseAddress, clientCredentials), cancellationToken);

                OutputHelper.WriteMessages(_outputDirName, identities);
                Logger.LogInformation("Messages created");

                await Mediator.Send(new CreateDatawalletModifications.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);

                OutputHelper.WriteDatawalletModifications(_outputDirName, identities);
                Logger.LogInformation("DatawalletModifications created");
            }
            catch (Exception e)
            {
                return new StatusMessage(false, e.Message);
            }


            return new StatusMessage(true, "Pool configuration with relationships and messages created successfully.");
        }
    }
}

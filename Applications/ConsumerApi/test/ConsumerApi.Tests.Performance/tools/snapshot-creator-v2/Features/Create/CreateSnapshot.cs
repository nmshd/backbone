using Backbone.ConsumerApi.Sdk.Authentication;
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
        IMediator Mediator)
        : IRequestHandler<Command, StatusMessage>
    {
        public async Task<StatusMessage> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation("Creating pool configuration with relationships and messages ...");

                var poolConfig = await PoolConfigurationJsonReader.Read(request.JsonFilePath);
                var clientCredentials = new ClientCredentials(request.ClientId, request.ClientSecret);

                var identities = await Mediator.Send(new CreateIdentities.Command(poolConfig.IdentityPoolConfigurations, request.BaseAddress, clientCredentials), cancellationToken);
                Logger.LogInformation("Identities created");

                identities = await Mediator.Send(new AddDevices.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                Logger.LogInformation("Devices added");

                identities = await Mediator.Send(new CreateRelationshipTemplates.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);
                Logger.LogInformation("Relationship templates created");

                identities = await Mediator.Send(new CreateRelationships.Command(poolConfig.RelationshipAndMessages, identities, request.BaseAddress, clientCredentials), cancellationToken);
                Logger.LogInformation("Relationships created");

                identities = await Mediator.Send(new CreateChallenges.Command(identities, request.BaseAddress, clientCredentials), cancellationToken);

                // Create Messages

                // Create DatawalletModifications


                Logger.LogInformation("Pool configuration with relationships and messages created successfully.");
            }
            catch (Exception e)
            {
                return new StatusMessage(false, e.Message);
            }


            return new StatusMessage(true, "Pool configuration with relationships and messages created successfully.");
        }
    }
}

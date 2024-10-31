using System;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Mediator;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;

public class PoolConfigurationSnapshotCreatorCommand(
    ILogger<PoolConfigurationSnapshotCreatorCommand> logger,
    IPerformanceTestConfigurationJsonReader performanceTestConfigurationJsonReader,
    IMediator mediator)
    : ICommand<PoolConfigurationSnapshotCreatorCommandArgs, StatusMessage>
{
    private PerformanceTestConfiguration _poolConfig;
    private ClientCredentials _clientCredentials;

    public async Task<StatusMessage> Execute(PoolConfigurationSnapshotCreatorCommandArgs parameter)
    {
        try
        {
            logger.LogInformation("Creating pool configuration with relationships and messages ...");

            _poolConfig = await performanceTestConfigurationJsonReader.Read(parameter.JsonFilePath);
            _clientCredentials = new ClientCredentials(parameter.ClientId, parameter.ClientSecret);

            var identities = await mediator.Send(new CreateIdentities.Command(_poolConfig.IdentityPoolConfigurations, parameter.BaseAddress, _clientCredentials));
            logger.LogInformation("Identities created");

            identities = await mediator.Send(new AddDevices.Command(identities, parameter.BaseAddress, _clientCredentials));
            logger.LogInformation("Devices added");

            identities = await mediator.Send(new CreateRelationshipTemplates.Command(identities, parameter.BaseAddress, _clientCredentials));
            logger.LogInformation("Relationship templates created");

            identities = await mediator.Send(new CreateRelationships.Command(_poolConfig.RelationshipAndMessages, identities, parameter.BaseAddress, _clientCredentials));
            logger.LogInformation("Relationships created");

            // Create Challenges

            // Create Messages

            // Create DatawalletModifications


            logger.LogInformation("Pool configuration with relationships and messages created successfully.");
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message);
        }


        return new StatusMessage(true, "Pool configuration with relationships and messages created successfully.");
    }
}

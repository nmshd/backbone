using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Mediator;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;

public class PoolConfigurationSnapshotCreatorCommand(
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
            _poolConfig = await performanceTestConfigurationJsonReader.Read(parameter.JsonFilePath);

            _clientCredentials = new ClientCredentials(parameter.ClientId, parameter.ClientSecret);
            var identities = await mediator.Send(new CreateIdentities.Command(_poolConfig.IdentityPoolConfigurations, parameter.BaseAddress, _clientCredentials));

            identities = await mediator.Send(new AddDevices.Command(identities, parameter.BaseAddress, _clientCredentials));

            // Create RelationshipTemplates


            // Create Relationships

            // Create Challenges

            // Create Messages

            // Create DatawalletModifications
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message);
        }


        return new StatusMessage(true, "Pool configuration with relationships and messages created successfully.");
    }
}

using System.CommandLine;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Application.Printer;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.EntityCreation;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsFile;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsGenerator;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator;

public class CreateEntitiesCommand : Command
{
    public CreateEntitiesCommand() : base("CreateEntities", "Creates entities in the consumer API using the SDK")
    {
        var baseAddress = new Option<string>("--baseAddress") { Description = "The base address of the consumer API.", Required = true };
        Options.Add(baseAddress);

        var clientId = new Option<string>("--clientId") { Description = "The client id to use.", Required = true };
        Options.Add(clientId);

        var clientSecret = new Option<string>("--clientSecret") { Description = "The corresponding client secret.", Required = true };
        Options.Add(clientSecret);

        var configurationFilePath = new Option<string>("--relationshipsAndMessages") { Description = "The csv file with the relationships and messages configuration.", Required = true };
        Options.Add(configurationFilePath);

        var poolsFilePath = new Option<string>("--poolsFile") { Description = "The json file with the pools' configuration.", Required = true };
        Options.Add(poolsFilePath);

        SetAction(async (parseResult, token) =>
        {
            var baseAddressValue = parseResult.GetRequiredValue(baseAddress);
            var clientIdValue = parseResult.GetRequiredValue(clientId);
            var clientSecretValue = parseResult.GetRequiredValue(clientSecret);
            var configurationFilePathValue = parseResult.GetRequiredValue(configurationFilePath);
            var poolsFilePathValue = parseResult.GetRequiredValue(poolsFilePath);

            await EntityCreationProcessor(baseAddressValue, clientIdValue, clientSecretValue, configurationFilePathValue, poolsFilePathValue);
        });
    }

    private static async Task EntityCreationProcessor(string baseAddress, string clientId, string clientSecret, string configurationFilePath, string poolsFilePath)
    {
        var poolsConfiguration = await Reader.ReadPools(poolsFilePath);
        var relationshipsAndMessages = await ReadRelationshipsAndMessagesConfigurationFile(configurationFilePath);

        var creator = new EntityCreator(baseAddress, clientId, clientSecret, poolsConfiguration.Pools.ToList(), relationshipsAndMessages, new Printer());
        await creator.StartCreation();
    }

    private static async Task<SolutionRepresentation> ReadRelationshipsAndMessagesConfigurationFile(string configurationFilePath)
    {
        var res = new SolutionRepresentation();

        using var reader = new StreamReader(configurationFilePath);
        await reader.ReadLineAsync(); // read header line
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line is null) break;
            var values = line.Split(';');
            uint from, to, count;
            if (values.Length == 3)
            {
                from = Convert.ToUInt32(values[0]);
                to = Convert.ToUInt32(values[1]);
                count = Convert.ToUInt32(values[2]);
            }
            else
            {
                from = Convert.ToUInt32(values[0]);
                to = Convert.ToUInt32(values[2]);
                count = Convert.ToUInt32(values[4]);
            }

            res.EstablishRelationship(from, to);
            for (uint i = 0; i < count; i++)
            {
                res.SendMessage(from, to);
            }
        }

        return res;
    }
}

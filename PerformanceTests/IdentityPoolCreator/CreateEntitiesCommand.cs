using System.CommandLine;
using Backbone.Identity.Pool.Creator.Application.Printer;
using Backbone.Identity.Pool.Creator.EntityCreation;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Identity.Pool.Creator.PoolsGenerator;

namespace Backbone.Identity.Pool.Creator;

public class CreateEntitiesCommand : Command
{
    public CreateEntitiesCommand() : base("CreateEntities", "Creates entities in the consumer API using the SDK")
    {
        var baseAddress = new Option<string>(name: "--baseAddress", description: "The base address of the consumer API.");
        AddOption(baseAddress);

        var clientId = new Option<string>(name: "--clientId", description: "The client id to use.");
        AddOption(clientId);

        var clientSecret = new Option<string>(name: "--clientSecret", description: "The corresponding client secret.");
        AddOption(clientSecret);

        var configurationFilePath = new Option<string>(name: "--ram", description: "The csv file with the relationships and messages configuration.");
        AddOption(configurationFilePath);

        var poolsFilePath = new Option<string>(name: "--poolsFile", description: "The json file with the pools' configuration.");
        AddOption(poolsFilePath);

        this.SetHandler(EntityCreationProcessor, baseAddress, clientId, clientSecret, configurationFilePath, poolsFilePath);
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

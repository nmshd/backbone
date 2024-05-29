using System.CommandLine;
using System.Text.Json;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.Tooling;
using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator;

public class MainCommand : RootCommand
{
    public MainCommand()
    {
        AddCommand(new GenerateCommand());
    }
}

public class GenerateCommand : Command
{
    public GenerateCommand() : base("GeneratePools", "Generates Pools according to the configuration provided")
    {
        var baseAddress = new Option<string>(name: "--baseAddress", description: "The base address of the consumer API.");
        AddOption(baseAddress);

        var clientId = new Option<string>(name: "--clientId", description: "The client id to use.");
        AddOption(clientId);

        var clientSecret = new Option<string>(name: "--clientSecret", description: "The corresponding client secret.");
        AddOption(clientSecret);

        var poolsFilePath = new Option<string>(name: "--poolsFile", description: "The json file with the pools configuration.");
        AddOption(poolsFilePath);



        this.SetHandler(Generate, baseAddress, clientId, clientSecret, poolsFilePath);

    }

    private async Task Generate(string baseAddress, string clientId, string clientSecret, string poolsFilePath)
    {
        var pools = await ReadPools(poolsFilePath);
        try
        {
            PoolsAreValid(pools);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine($"Pool Parsing failed. Error: {e.Message}");
        }

        var password = PasswordHelper.GeneratePassword(18, 24);
        var sdk = await Client.CreateForNewIdentity(baseAddress, new ClientCredentials(clientId, clientSecret), password);

        var credentials = sdk.DeviceData?.UserCredentials;
    }

    /// <summary>
    /// Validation is a simple process. We split the pools up into two groups: app and connector.
    /// The total of Messages & Relationships in each group must match.
    /// </summary>
    /// <param name="pools"></param>
    private static void PoolsAreValid(PoolEntry[] pools)
    {
        var appPools = pools.Where(p => p.Type == "app").ToList();
        var connectorPools = pools.Where(p => p.Type == "connector").ToList();

        var appMessagesSum = appPools.Sum(p => p.NumberOfSentMessages * p.Amount);
        var appRelationshipsSum = appPools.Sum(p => p.NumberOfRelationships * p.Amount);
        
        var connectorMessagesSum = connectorPools.Sum(p => p.NumberOfSentMessages * p.Amount);
        var connectorRelationshipsSum = connectorPools.Sum(p => p.NumberOfRelationships * p.Amount);

        if (appMessagesSum != connectorMessagesSum)
            throw new ArgumentException($"The number of messages for app pools and connector pools does not match (difference is {long.Abs(appMessagesSum - connectorMessagesSum)}).");

        if(appRelationshipsSum != connectorRelationshipsSum)
            throw new ArgumentException($"The number of relationships for app pools and connector pools does not match (difference is {long.Abs(appRelationshipsSum - connectorRelationshipsSum)}).");
    }

    private static async Task<PoolEntry[]> ReadPools(string poolsFilePath)
    {
        var poolsFile = await File.ReadAllBytesAsync(poolsFilePath);
        var pools = JsonSerializer.Deserialize<PoolEntry[]>(poolsFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return pools ?? throw new Exception($"Could not read {poolsFilePath}.");
    }
}


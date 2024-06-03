using System.CommandLine;
using System.Text.Json;
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

        var poolsFilePath = new Option<string>(name: "--poolsFile", description: "The json file with the pools' configuration.");
        AddOption(poolsFilePath);

        this.SetHandler(GenerationPreprocessor, baseAddress, clientId, clientSecret, poolsFilePath);
    }

    private static async Task GenerationPreprocessor(string baseAddress, string clientId, string clientSecret, string poolsFilePath)
    {
        var pools = await ReadPools(poolsFilePath);
        var generator = new PoolsGenerator.PoolsGenerator(baseAddress, clientId, clientSecret, pools);
        await generator.CreatePools();
    }


    private static async Task<PoolEntry[]> ReadPools(string poolsFilePath)
    {
        var poolsFile = await File.ReadAllBytesAsync(poolsFilePath);
        var pools = JsonSerializer.Deserialize<PoolEntry[]>(poolsFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return pools ?? throw new Exception($"Could not read {poolsFilePath}.");
    }
}


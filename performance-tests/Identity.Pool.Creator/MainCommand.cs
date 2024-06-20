using System.CommandLine;
using System.Text.Json;
using Backbone.Identity.Pool.Creator.Application.MessageDistributor;
using Backbone.Identity.Pool.Creator.Application.Printer;
using Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;
using Backbone.Identity.Pool.Creator.GraphPoolsGenerator;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Microsoft.Extensions.DependencyInjection;

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
        var serviceProvider = ConfigureServices();

        var poolsConfiguration = await ReadPools(poolsFilePath);
        var generator = new PoolsGenerator.PoolsGenerator(baseAddress, 
            clientId, 
            clientSecret, 
            poolsConfiguration,
            serviceProvider.GetRequiredService<IRelationshipDistributor>(),
            serviceProvider.GetRequiredService<IMessageDistributor>(),
            printer: serviceProvider.GetRequiredService<IPrinter>()

            );

        var graphGenerator = new GraphPoolsGenerator.GraphPoolsGenerator(
            baseAddress,
            clientId,
            clientSecret,
            poolsConfiguration,
            printer: serviceProvider.GetRequiredService<IPrinter>()
        );

        //await graphGenerator.CreatePools();
        await generator.CreatePools();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IRelationshipDistributor, RelationshipDistributorV3>();

        services.AddSingleton<IMessageDistributor, MessageDistributorV2>();

        services.AddSingleton<IPrinter, Printer>();

        return services.BuildServiceProvider();
    }


    private static async Task<PoolFileRoot> ReadPools(string poolsFilePath)
    {
        var poolsFile = await File.ReadAllBytesAsync(poolsFilePath);

        var poolsConfiguration = JsonSerializer.Deserialize<PoolFileRoot>(poolsFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return poolsConfiguration ?? throw new Exception($"Could not read {poolsFilePath}.");
    }
}


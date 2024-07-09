using System.CommandLine;
using System.Text.Json;
using Backbone.Identity.Pool.Creator.Application.MessageDistributor;
using Backbone.Identity.Pool.Creator.Application.Printer;
using Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Identity.Pool.Creator.PoolsGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Identity.Pool.Creator;

public class GeneratePoolsCommand : Command
{
    public GeneratePoolsCommand() : base("GeneratePools", "Generates Pools according to the configuration provided")
    {
        var poolsFilePath = new Option<string>(name: "--poolsFile", description: "The json file with the pools' configuration.");
        AddOption(poolsFilePath);

        this.SetHandler(GenerationPreprocessor, poolsFilePath);
    }

    private static async Task GenerationPreprocessor(string poolsFilePath)
    {
        var serviceProvider = ConfigureServices();

        var poolsConfiguration = await Reader.ReadPools(poolsFilePath);
        var generator = new PoolsGenerator.PoolsGenerator(poolsConfiguration,
            serviceProvider.GetRequiredService<IRelationshipDistributor>(),
            serviceProvider.GetRequiredService<IMessageDistributor>(),
            printer: serviceProvider.GetRequiredService<IPrinter>()

        );

        await generator.CreatePools();

        var saGenerator = new SimulatedAnnealingPoolsGenerator(poolsConfiguration, printer: serviceProvider.GetRequiredService<IPrinter>());
        await saGenerator.CreatePools();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IRelationshipDistributor, RelationshipDistributorV4>();

        services.AddSingleton<IMessageDistributor, MessageDistributorV2>();

        services.AddSingleton<IPrinter, Printer>();

        return services.BuildServiceProvider();
    }
}

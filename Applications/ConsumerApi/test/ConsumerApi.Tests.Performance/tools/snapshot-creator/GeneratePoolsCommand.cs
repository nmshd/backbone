using System.CommandLine;
using Backbone.PerformanceSnapshotCreator.Application.MessageDistributor;
using Backbone.PerformanceSnapshotCreator.Application.Printer;
using Backbone.PerformanceSnapshotCreator.Application.RelationshipDistributor;
using Backbone.PerformanceSnapshotCreator.PoolsFile;
using Backbone.PerformanceSnapshotCreator.PoolsGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.PerformanceSnapshotCreator;

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

        new DeterministicPoolsGenerator(poolsConfiguration,
            serviceProvider.GetRequiredService<IRelationshipDistributor>(),
            serviceProvider.GetRequiredService<IMessageDistributor>(),
            serviceProvider.GetRequiredService<IPrinter>()
        ).CreatePools();

        new SimulatedAnnealingPoolsGenerator(poolsConfiguration,
            serviceProvider.GetRequiredService<IPrinter>()
        ).CreatePools();
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

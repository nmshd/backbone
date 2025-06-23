using System.CommandLine;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Application.MessageDistributor;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Application.Printer;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Application.RelationshipDistributor;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsFile;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator;

public class GeneratePoolsCommand : Command
{
    public GeneratePoolsCommand() : base("GeneratePools", "Generates Pools according to the configuration provided")
    {
        var poolsFilePath = new Option<string>("--poolsFile") { Description = "The json file with the pools' configuration.", Required = true };
        Options.Add(poolsFilePath);

        SetAction(async (parseResult, token) =>
        {
            var poolsFilePathValue = parseResult.GetRequiredValue(poolsFilePath);
            await GenerationPreprocessor(poolsFilePathValue);
        });
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

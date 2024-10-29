using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands.Args;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Generator;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Validators;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Writers;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2;

public class Program
{
    private static Task<int> Main(string[] args)
    {
        return new HostBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IPerformanceTestConfigurationExcelReader, PerformanceTestConfigurationExcelReader>();
                services.AddSingleton<IPoolConfigurationJsonWriter, PoolConfigurationJsonWriter>();
                services.AddSingleton<IPerformanceTestConfigurationJsonReader, PerformanceTestConfigurationJsonReader>();
                services.AddSingleton<IPoolConfigurationJsonValidator, PoolConfigurationJsonValidator>();
                services.AddSingleton<IRelationshipAndMessagesExcelWriter, RelationshipAndMessagesExcelWriter>();
                services.AddSingleton<IRelationshipAndMessagesGenerator, RelationshipAndMessagesGenerator>();

                services.AddSingleton<PoolConfigurationJsonGeneratorCommand>();
                services.AddSingleton<PoolConfigurationJsonValidatorCommand>();
                services.AddSingleton<RelationshipAndMessagesGeneratorCommand>();
                services.AddSingleton<PoolConfigWithRelationshipAndMessagesGeneratorCommand>();
            })
            .RunCommandLineApplicationAsync(args, app =>
            {
                app.Command("generate-json", command =>
                {
                    command.Description = "Generate JSON Pool Config";
                    var sourceOption = command.Option<string>("-s|--source <SOURCE>", "Source Excel File", CommandOptionType.SingleValue);
                    var worksheetOption = command.Option<string>("-w|--worksheet <WORKSHEET>", "Excel Worksheet Name", CommandOptionType.SingleValue);

                    command.OnExecuteAsync(async _ =>
                    {
                        var excelFilePath = sourceOption.Value();
                        var workSheetName = worksheetOption.Value();

                        var poolConfigJsonGeneratorCommand = app.GetRequiredService<PoolConfigurationJsonGeneratorCommand>();

                        var result = await poolConfigJsonGeneratorCommand.Execute(new PoolConfigurationJsonGeneratorCommandArgs(excelFilePath!, workSheetName!));

                        Console.WriteLine(result.Status ? $"Pool Config JSON generated at {result.Message}" : $"Error: {result.Message}");

                        return 0;
                    });
                });

                app.Command("verify-json", command =>
                {
                    command.Description = "Verify JSON Pool Config";
                    var sourceOption = command.Option<string>("-s|--source <SOURCE>", "Source Excel File", CommandOptionType.SingleValue);
                    var worksheetOption = command.Option<string>("-w|--worksheet <WORKSHEET>", "Excel Worksheet Name", CommandOptionType.SingleValue);
                    var poolConfigOption = command.Option<string>("-p|--pool-config <POOLCONFIG>", "Pool Config JSON File", CommandOptionType.SingleValue);

                    command.OnExecuteAsync(async _ =>
                    {
                        var excelFilePath = sourceOption.Value();
                        var worksheetName = worksheetOption.Value();
                        var jsonFilePath = poolConfigOption.Value();

                        var poolConfigJsonValidatorCommand = app.GetRequiredService<PoolConfigurationJsonValidatorCommand>();

                        var result = await poolConfigJsonValidatorCommand.Execute(new PoolConfigurationJsonValidatorCommandArgs(excelFilePath!, worksheetName!, jsonFilePath!));

                        Console.WriteLine(result ? "Pool Config JSON is valid" : "Pool Config JSON is invalid");

                        return 0;
                    });
                });

                app.Command("generate-relationships", command =>
                {
                    command.Description = "Generate Relationships and Messages Excel";
                    var poolsFileOption = command.Option<string>("-p|--pool-config <POOLCONFIG>", "Pool Config JSON File", CommandOptionType.SingleValue);
                    var worksheetOption = command.Option<string>("-w|--worksheet <WORKSHEET>", "Excel Worksheet Name", CommandOptionType.SingleValue);

                    command.OnExecuteAsync(async _ =>
                    {
                        var poolsFile = poolsFileOption.Value();
                        var worksheetName = worksheetOption.Value();

                        var relationshipAndMessagesGeneratorCommand = app.GetRequiredService<RelationshipAndMessagesGeneratorCommand>();

                        var result = await relationshipAndMessagesGeneratorCommand.Execute(new RelationshipAndMessagesGeneratorCommandArgs(poolsFile!, worksheetName!));

                        Console.WriteLine(result.Status ? $"Relationships and Messages Excel generated at {result.Message}" : $"Error: {result.Message}");

                        return 0;
                    });
                });

                app.Command("generate-all", command =>
                {
                    command.Description = "Generate JSON Pool Config including all Relationships and Messages in a single JSON File";
                    var sourceOption = command.Option<string>("-s|--source <SOURCE>", "Source Excel File", CommandOptionType.SingleValue);
                    var worksheetOption = command.Option<string>("-w|--worksheet <WORKSHEET>", "Excel Worksheet Name", CommandOptionType.SingleValue);

                    command.OnExecuteAsync(async _ =>
                    {
                        var excelFilePath = sourceOption.Value();
                        var workSheetName = worksheetOption.Value();

                        var poolConfigWithRelationshipAndMessagesGeneratorCommand = app.GetRequiredService<PoolConfigWithRelationshipAndMessagesGeneratorCommand>();

                        var result = await poolConfigWithRelationshipAndMessagesGeneratorCommand.Execute(new PoolConfigWithRelationshipAndMessagesGeneratorCommandArgs(excelFilePath!, workSheetName!));

                        Console.WriteLine(result.Status ? $"Pool Configs with Relationships and Messages JSON generated at {result.Message}" : $"Error: {result.Message}");

                        return 0;
                    });
                });

                app.OnExecute(() => Console.WriteLine("No command provided. add --help for guidance."));
            });
    }
}

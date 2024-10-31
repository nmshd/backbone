using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;
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
                services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Program>());

                services.AddSingleton<IPerformanceTestConfigurationExcelReader, PerformanceTestConfigurationExcelReader>();
                services.AddSingleton<IPoolConfigurationJsonWriter, PoolConfigurationJsonWriter>();
                services.AddSingleton<IPerformanceTestConfigurationJsonReader, PerformanceTestConfigurationJsonReader>();
                services.AddSingleton<IPoolConfigurationJsonValidator, PoolConfigurationJsonValidator>();

                services.AddSingleton<IRelationshipAndMessagesGenerator, RelationshipAndMessagesGenerator>();

                services.AddSingleton<PoolConfigurationValidatorCommand>();
                services.AddSingleton<PoolConfigurationGeneratorCommand>();
                services.AddSingleton<PoolConfigurationSnapshotCreatorCommand>();
            })
            .RunCommandLineApplicationAsync(args, app =>
            {
                app.Command("verify-config", command =>
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

                        var poolConfigJsonValidatorCommand = app.GetRequiredService<PoolConfigurationValidatorCommand>();

                        var result = await poolConfigJsonValidatorCommand.Execute(new PoolConfigurationValidatorCommandArgs(excelFilePath!, worksheetName!, jsonFilePath!));

                        Console.WriteLine(result ? "Pool Config JSON is valid" : "Pool Config JSON is invalid");

                        return 0;
                    });
                });

                app.Command("generate-config", command =>
                {
                    command.Description = "Generate JSON Pool Config including all Relationships and Messages in a single JSON File";
                    var sourceOption = command.Option<string>("-s|--source <SOURCE>", "Source Excel File", CommandOptionType.SingleValue);
                    var worksheetOption = command.Option<string>("-w|--worksheet <WORKSHEET>", "Excel Worksheet Name", CommandOptionType.SingleValue);

                    command.OnExecuteAsync(async _ =>
                    {
                        var excelFilePath = sourceOption.Value();
                        var workSheetName = worksheetOption.Value();

                        var poolConfigWithRelationshipAndMessagesGeneratorCommand = app.GetRequiredService<PoolConfigurationGeneratorCommand>();

                        var result = await poolConfigWithRelationshipAndMessagesGeneratorCommand.Execute(new PoolConfigurationGeneratorCommandArgs(excelFilePath!, workSheetName!));

                        Console.WriteLine(result.Status ? $"Pool Configs with Relationships and Messages JSON generated at {result.Message}" : $"Error: {result.Message}");

                        return 0;
                    });
                });

                app.Command("create-snapshot", command =>
                {
                    command.Description = "Apply Pool Config with Relationships and Messages to the Consumer API";
                    var baseAddressOption = command.Option<string>("-b|--baseAddress <BASEADDRESS>", "Base Address of the Consumer API", CommandOptionType.SingleValue);
                    var clientIdOption = command.Option<string>("-c|--clientId <CLIENTID>", "Client Id of the Consumer API", CommandOptionType.SingleValue);
                    var clientSecretOption = command.Option<string>("-s|--clientSecret <CLIENTSECRET>", "Client Secret of the Consumer API", CommandOptionType.SingleValue);
                    var poolConfigOption = command.Option<string>("-p|--pool-config <POOLCONFIG>", "Pool Config JSON File", CommandOptionType.SingleValue);

                    command.OnExecuteAsync(async _ =>
                    {
                        var baseAddress = baseAddressOption.Value();
                        var clientId = clientIdOption.Value();
                        var clientSecret = clientSecretOption.Value();
                        var poolConfigJsonFilePath = poolConfigOption.Value();

                        var poolConfigWithRelationshipAndMessagesCreatorCommand = app.GetRequiredService<PoolConfigurationSnapshotCreatorCommand>();

                        var result = await poolConfigWithRelationshipAndMessagesCreatorCommand.Execute(
                            new PoolConfigurationSnapshotCreatorCommandArgs(baseAddress!, clientId!, clientSecret!, poolConfigJsonFilePath!));

                        Console.WriteLine(result.Status ? "Snapshot created successfully" : $"Error: {result.Message}");
                    });
                });

                app.OnExecute(() => Console.WriteLine("No command provided. add --help for guidance."));
            });
    }
}

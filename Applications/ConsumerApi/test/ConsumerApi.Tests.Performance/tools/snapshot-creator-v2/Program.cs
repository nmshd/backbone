using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Writers;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2;

public class Program
{
    static Task<int> Main(string[] args)
        => new HostBuilder()
            .ConfigureLogging((context, logging) => { logging.AddConsole(); })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<PerformanceTestConfigurationExcelReader>();
                services.AddSingleton<PoolConfigurationJsonWriter>();


                services.AddSingleton<PoolConfigurationJsonGeneratorCommand>();
            })
            .RunCommandLineApplicationAsync(args, (app) =>
            {
                app.Command("generate-json", (command) =>
                {
                    command.Description = "Generate JSON Pool Config";
                    var sourceOption = command.Option<string>("--source <SOURCE>", "Source Excel File", CommandOptionType.SingleValue);
                    var worksheetOption = command.Option<string>("--worksheet <WORKSHEET>", "Worksheet Name", CommandOptionType.SingleValue);
                    command.OnExecuteAsync(async (cancellationToken) =>
                    {
                        var excelFilePath = sourceOption.Value();
                        var workSheetName = worksheetOption.Value();

                        var poolConfigJsonGeneratorCommand = app.GetRequiredService<PoolConfigurationJsonGeneratorCommand>();

                        var result = await poolConfigJsonGeneratorCommand.Execute(new PoolConfigurationJsonGeneratorCommandArgs(excelFilePath, workSheetName));

                        Console.WriteLine(result.Status ? $"Pool Config JSON generated at {result.Message}" : $"Error: {result.Message}");

                        return 0;
                    });
                });
            });
}

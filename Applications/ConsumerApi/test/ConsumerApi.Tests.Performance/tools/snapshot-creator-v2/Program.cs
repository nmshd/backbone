using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2;

public class Program
{
    private static async Task<int> Main(string[] args)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configurationBuilder)
            .CreateLogger();

        try
        {
            Log.Information("Starting up");

            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((_, config) => { config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); })
                .ConfigureLogging(configureLogging =>
                {
                    configureLogging.ClearProviders();
                    configureLogging.AddSerilog();
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Program>());
                    services.AddSingleton<IPoolConfigurationExcelReader, PoolConfigurationExcelReader>();
                    services.AddSingleton<IPoolConfigurationJsonWriter, PoolConfigurationJsonWriter>();
                    services.AddSingleton<IPoolConfigurationJsonReader, PoolConfigurationJsonReader>();
                    services.AddSingleton<IPoolConfigurationJsonValidator, PoolConfigurationJsonValidator>();
                    services.AddSingleton<IRelationshipAndMessagesGenerator, RelationshipAndMessagesGenerator>();
                    services.AddSingleton<IOutputHelper, OutputHelper>();
                    services.AddSingleton<IExcelWriter, ExcelWriter>();

                    services.AddTransient<ICreateIdentityCommand, IdentityCommand>();
                });

            await hostBuilder.RunCommandLineApplicationAsync(args, app =>
            {
                app.Command("verify-config", VerifyConfigCommand);
                app.Command("generate-config", GenerateConfigCommand);
                app.Command("create-snapshot", CreateSnapshotCommand);

                app.OnExecute(() => Console.WriteLine("No command provided. add --help for guidance."));
            });
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }

        return 0;
    }

    private static void VerifyConfigCommand(CommandLineApplication command)
    {
        command.Description = "Verify JSON Pool Config";
        var sourceOption = command.Option<string>("-s|--source <SOURCE>", "Source Excel File", CommandOptionType.SingleValue);
        var worksheetOption = command.Option<string>("-w|--worksheet <WORKSHEET>", "Excel Worksheet Name", CommandOptionType.SingleValue);
        var poolConfigOption = command.Option<string>("-p|--pool-config <POOLCONFIG>", "Pool Config JSON File", CommandOptionType.SingleValue);

        command.OnExecuteAsync(async cancellationToken =>
        {
            var excelFilePath = sourceOption.Value();
            var worksheetName = worksheetOption.Value();
            var jsonFilePath = poolConfigOption.Value();

            var mediator = command.GetRequiredService<IMediator>();
            var logger = command.GetRequiredService<ILogger<VerifyConfig>>();

            var result = await mediator.Send(new VerifyConfig.Command(excelFilePath!, worksheetName!, jsonFilePath!), cancellationToken);

            logger.LogInformation("Pool Config {JsonConfigFile} is {Status}", jsonFilePath, result ? "valid" : "invalid");

            return 0;
        });
    }

    private static void GenerateConfigCommand(CommandLineApplication command)
    {
        command.Description = "Generate JSON Pool Config including all Relationships and Messages in a single JSON File";
        var sourceOption = command.Option<string>("-s|--source <SOURCE>", "Source Excel File", CommandOptionType.SingleValue);
        var worksheetOption = command.Option<string>("-w|--worksheet <WORKSHEET>", "Excel Worksheet Name", CommandOptionType.SingleValue);
        var debugModeOption = command.Option<bool>("-d|--debug", "Debug Mode. If enabled, the pool config is generated as an Excel file for easier readability.",
            CommandOptionType.NoValue);

        command.OnExecuteAsync(async cancellationToken =>
        {
            var excelFilePath = sourceOption.Value();
            var workSheetName = worksheetOption.Value();
            var debugMode = debugModeOption.ParsedValue;

            var mediator = command.GetRequiredService<IMediator>();
            var logger = command.GetRequiredService<ILogger<GenerateConfig>>();

            var result = await mediator.Send(new GenerateConfig.Command(excelFilePath!, workSheetName!, debugMode), cancellationToken);

            if (result.Status)
            {
                logger.LogInformation("Pool Configs with Relationships and Messages JSON {WithDebugModeExcel} generated at {Path}",
                    debugMode ? "and EXCEL" : string.Empty,
                    Path.GetDirectoryName(result.Message));
            }
            else
            {
                logger.LogError(result.Exception, "Error: {Message}", result.Message);
            }

            return 0;
        });
    }

    private static void CreateSnapshotCommand(CommandLineApplication command)
    {
        command.Description = "Apply Pool Config with Relationships and Messages to the Consumer API";
        var baseAddressOption = command.Option<string>("-b|--baseAddress <BASEADDRESS>", "Base Address of the Consumer API", CommandOptionType.SingleValue);
        var clientIdOption = command.Option<string>("-c|--clientId <CLIENTID>", "Client Id of the Consumer API", CommandOptionType.SingleValue);
        var clientSecretOption = command.Option<string>("-s|--clientSecret <CLIENTSECRET>", "Client Secret of the Consumer API", CommandOptionType.SingleValue);
        var poolConfigOption = command.Option<string>("-p|--pool-config <POOLCONFIG>", "Pool Config JSON File", CommandOptionType.SingleValue);

        command.OnExecuteAsync(async cancellationToken =>
        {
            var baseAddress = baseAddressOption.Value() ?? "http://localhost:8081";
            var clientId = clientIdOption.Value();
            var clientSecret = clientSecretOption.Value();
            var poolConfigJsonFilePath = poolConfigOption.Value();

            var mediator = command.GetRequiredService<IMediator>();
            var logger = command.GetRequiredService<ILogger<CreateSnapshot>>();

            var result = await mediator.Send(new CreateSnapshot.Command(baseAddress, clientId!, clientSecret!, poolConfigJsonFilePath!), cancellationToken);

            if (result.Status)
            {
                logger.LogInformation("Snapshot created successfully");
            }
            else
            {
                logger.LogError(result.Exception, "Error: {Message}", result.Message);
            }

            return 0;
        });
    }
}

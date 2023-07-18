using System.CommandLine;
using System.Text.Json;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.AdminCli;

public class Program
{
    private const string SQL_SERVER = "SqlServer";
    private const string SQL_SERVER_MIGRATIONS_ASSEMBLY = "Devices.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Devices.Infrastructure.Database.Postgres";

    private static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS =
        new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private static readonly Option<string> DB_CONNECTION_STRING_OPTION =
        new("-c", "The connection string to the database.");

    private static readonly Option<string> DB_PROVIDER_OPTION =
        new("-p", "The database provider. Possible values: Postgres, SqlServer");

    private static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand();

        DB_PROVIDER_OPTION.AddAlias("--dbProvider");
        DB_PROVIDER_OPTION.SetDefaultValueFactory(GetDbProviderFromEnvVar);
        DB_PROVIDER_OPTION.AddValidator(result =>
        {
            result.ErrorMessage = ValidateProvider(result.GetValueOrDefault<string>());
        });

        DB_CONNECTION_STRING_OPTION.AddAlias("--dbConnectionString");
        DB_CONNECTION_STRING_OPTION.SetDefaultValueFactory(GetDbConnectionStringFromEnvVar);
        DB_CONNECTION_STRING_OPTION.AddValidator(result =>
        {
            result.ErrorMessage = ValidateDbConnectionString(result.GetValueOrDefault<string>());
        });

        rootCommand.AddOption(DB_CONNECTION_STRING_OPTION);
        rootCommand.AddOption(DB_PROVIDER_OPTION);

        rootCommand.AddCommand(ClientCommand);
        await rootCommand.InvokeAsync(args);
    }

    private static Command ClientCommand
    {
        get
        {
            var command = new Command("client");

            command.AddCommand(CreateClientCommand);
            command.AddCommand(ListClientsCommand);
            command.AddCommand(DeleteClientsCommand);
            return command;
        }
    }

    private static Command CreateClientCommand
    {
        get
        {
            var command = new Command("create", "Create an OAuth client")
            {
                Description = "Create an OAuth client"
            };

            var clientId = new Option<string>("--clientId")
            {
                IsRequired = false,
                Description = "The clientId of the OAuth client. Default: a randomly generated string."
            };
            var displayName = new Option<string>("--displayName")
            {
                IsRequired = false,
                Description = "The displayName of the OAuth client. Default: the clientId."
            };

            var clientSecret = new Option<string>("--clientSecret")
            {
                IsRequired = false,
                Description = "The clientSecret of the OAuth client. Default: a randomly generated string."
            };

            command.AddOption(DB_PROVIDER_OPTION);
            command.AddOption(DB_CONNECTION_STRING_OPTION);
            command.AddOption(clientId);
            command.AddOption(displayName);
            command.AddOption(clientSecret);

            command.SetHandler(CreateClient, DB_PROVIDER_OPTION, DB_CONNECTION_STRING_OPTION, clientId, displayName,
                clientSecret);

            return command;
        }
    }

    private static Command ListClientsCommand
    {
        get
        {
            var command = new Command("list")
            {
                Description = "List all existing OAuth clients"
            };

            command.AddOption(DB_PROVIDER_OPTION);
            command.AddOption(DB_CONNECTION_STRING_OPTION);

            command.SetHandler(ListClients, DB_PROVIDER_OPTION, DB_CONNECTION_STRING_OPTION);
            return command;
        }
    }

    private static Command DeleteClientsCommand
    {
        get
        {
            var command = new Command("delete")
            {
                Description = "Deletes the OAuth clients with the given clientId's"
            };

            var clientIds = new Argument<string[]>("clientIds")
            {
                Arity = ArgumentArity.OneOrMore,
                Description = "The clientId's that should be deleted."
            };

            command.AddOption(DB_PROVIDER_OPTION);
            command.AddOption(DB_CONNECTION_STRING_OPTION);
            command.AddArgument(clientIds);

            command.SetHandler(DeleteClients, DB_PROVIDER_OPTION, DB_CONNECTION_STRING_OPTION, clientIds);

            return command;
        }
    }

    private static async Task CreateClient(string? dbProvider, string? dbConnectionString, string? clientId,
        string? displayName, string? clientSecret)
    {
        var oAuthClientManager = GetOAuthClientManager(dbProvider!, dbConnectionString!);

        var createdClient = await oAuthClientManager.Create(clientId, displayName, clientSecret);

        Console.WriteLine(JsonSerializer.Serialize(createdClient, JSON_SERIALIZER_OPTIONS));
        Console.WriteLine("Please note the secret since you cannot obtain it later.");
    }

    private static async Task ListClients(string? dbProvider, string? dbConnectionString)
    {
        var oAuthClientManager = GetOAuthClientManager(dbProvider!, dbConnectionString!);

        var clients = oAuthClientManager.GetAll();
        Console.WriteLine("The following clients are configured:");

        await foreach (var client in clients)
        {
            Console.WriteLine(JsonSerializer.Serialize(client, JSON_SERIALIZER_OPTIONS));
        }
    }

    private static async Task DeleteClients(string? dbProvider, string? dbConnectionString, string[] clientIds)
    {
        var oAuthClientManager = GetOAuthClientManager(dbProvider!, dbConnectionString!);

        foreach (var clientId in clientIds)
        {
            try
            {
                await oAuthClientManager.Delete(clientId);
                Console.WriteLine($"Successfully deleted client '{clientId}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static OAuthClientManager GetOAuthClientManager(string dbProvider, string dbConnectionString)
    {
        var services = new ServiceCollection();
        ConfigureServices(services,
            new ApplicationConfiguration { Provider = dbProvider, DbConnectionString = dbConnectionString });

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<OAuthClientManager>();
    }

    private static void ConfigureServices(IServiceCollection services,
        ApplicationConfiguration applicationConfiguration)
    {
        switch (applicationConfiguration.Provider)
        {
            case SQL_SERVER:
                services.AddDbContext<DevicesDbContext>(options =>
                {
                    options.UseSqlServer(applicationConfiguration.DbConnectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(SQL_SERVER_MIGRATIONS_ASSEMBLY);
                    });

                    options.UseOpenIddict();
                });
                break;
            case POSTGRES:
                services.AddDbContext<DevicesDbContext>(options =>
                {
                    options.UseNpgsql(applicationConfiguration.DbConnectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                    });

                    options.UseOpenIddict();
                });
                break;
            default:
                throw new Exception($"Unsupported database provider: {applicationConfiguration.Provider}");
        }

        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<DevicesDbContext>();

        services
            .AddOpenIddict()
            .AddCore(options =>
            {
                options
                    .UseEntityFrameworkCore()
                    .UseDbContext<DevicesDbContext>();
            });

        services.AddLogging();
        services.AddTransient<OAuthClientManager>();
    }

    private static string? ValidateDbConnectionString(string? connectionString)
    {
        return connectionString.IsNullOrEmpty()
            ? "You need to specify a database connection string by passing it via an option (-c/--connectionString) or by setting it via environment variable (Database__ConnectionString/Database:ConnectionString)."
            : null;
    }

    private static string? ValidateProvider(string? provider)
    {
        return provider.IsNullOrEmpty()
            ? "You need to specify a database provider by passing it via an option (-p/--provider) or by setting it via environment variable (Database__Provider/Database:Provider)."
            : null;
    }

    private static string? GetDbProviderFromEnvVar()
    {
        var provider = Environment.GetEnvironmentVariable("Database__Provider");

        if (provider.IsNullOrEmpty())
            provider = Environment.GetEnvironmentVariable("Database:Provider");

        return provider;
    }

    private static string? GetDbConnectionStringFromEnvVar()
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString");

        if (connectionString.IsNullOrEmpty())
            connectionString = Environment.GetEnvironmentVariable("Database:ConnectionString");

        return connectionString;
    }
}

public class ApplicationConfiguration
{
    public string DbConnectionString { get; set; } = null!;
    public string Provider { get; set; } = null!;
}

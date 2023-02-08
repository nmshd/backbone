using System.Reflection;
using System.Text.Json;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.AdminCli;

public class ApplicationConfiguration
{
    public string DbConnectionString { get; set; } = null!;

    public void Validate()
    {
        if (string.IsNullOrEmpty(DbConnectionString))
            throw new Exception($"{nameof(DbConnectionString)} must not be empty.");
    }
}

public class Program
{
    private static OAuthClientManager _oAuthClientManager = null!;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    private static readonly ConsoleMenu Menu = new(new MenuItem[]
    {
        new(1, "Create client", CreateClient),
        new(2, "Create anonymous client", CreateAnonymousClient),
        new(3, "Delete client", DeleteClient),
        new(4, "List clients", ListClients),
        new(5, "Exit", Exit)
    });

    public static async Task<int> Main(string[] args)
    {
        ApplicationConfiguration configuration;
        try
        {
            configuration = GetConfiguration(args);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(e.Message);
            return 1;
        }

        var services = new ServiceCollection();
        ConfigureServices(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        _oAuthClientManager = serviceProvider.GetRequiredService<OAuthClientManager>();

        await Run();

        return 0;
    }

    private static void ConfigureServices(IServiceCollection services, ApplicationConfiguration applicationConfiguration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // Configure the context to use Microsoft SQL Server.
            options.UseSqlServer(applicationConfiguration.DbConnectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name);
            });

            // Register the entity sets needed by OpenIddict.
            options.UseOpenIddict();
        });

        // Register the Identity services.
        services.AddIdentity<ApplicationUser, IdentityRole>(config =>
        {
            config.Password.RequireDigit = false;
            config.Password.RequiredLength = 4;
            config.Password.RequireNonAlphanumeric = false;
            config.Password.RequireUppercase = false;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddOpenIddict()
            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>();
            });

        services.AddLogging();
        services.AddTransient<OAuthClientManager>();
    }

    private static ApplicationConfiguration GetConfiguration(string[] args)
    {
        var commandLineOptions = new ConfigurationBuilder().AddCommandLine(args, new Dictionary<string, string> { { "-c", "ConfigurationFile" } }).Build();
        var configurationFile = commandLineOptions.GetValue<string>("ConfigurationFile");

        var configurationBuilder =
            new ConfigurationBuilder()
                .AddEnvironmentVariables();

        if (!string.IsNullOrEmpty(configurationFile))
        {
            var fullPathToConfigurationFile = Path.Combine(Environment.CurrentDirectory, configurationFile);
            configurationBuilder = configurationBuilder.AddJsonFile(fullPathToConfigurationFile, true, false);
        }

        configurationBuilder = configurationBuilder.AddCommandLine(args);

        var configuration = configurationBuilder.Build();

        var applicationConfiguration = new ApplicationConfiguration
        {
            DbConnectionString = configuration.GetValue<string>("Database:ConnectionString")
        };

        applicationConfiguration.Validate();

        Console.WriteLine("The following configuration is used: ");
        Console.WriteLine(JsonSerializer.Serialize(applicationConfiguration, _jsonSerializerOptions));

        return applicationConfiguration;
    }

    private static async Task Run()
    {
        while (true)
        {
            var userChoice = Menu.AskForItemChoice();
            await userChoice.Action.Invoke();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static async Task CreateClient()
    {
        var clientId = ConsoleHelpers.ReadOptional("clientId (optional)");
        var clientName = ConsoleHelpers.ReadOptional("displayName (optional)");
        var clientSecret = ConsoleHelpers.ReadOptional("clientSecret (optional)");

        var createdClient = await _oAuthClientManager.Create(clientId, clientName, clientSecret);

        Console.WriteLine(JsonSerializer.Serialize(createdClient, _jsonSerializerOptions));
        Console.WriteLine("Please note the secret since you cannot obtain it later.");
    }

    private static async Task CreateAnonymousClient()
    {
        var createdClient = await _oAuthClientManager.Create(null, null, null);

        Console.WriteLine(JsonSerializer.Serialize(createdClient, _jsonSerializerOptions));
        Console.WriteLine("Please note the secret since you cannot obtain it later.");
    }

    private static async Task DeleteClient()
    {
        try
        {
            var clientId = ConsoleHelpers.ReadRequired("clientId");
            await _oAuthClientManager.Delete(clientId);
            Console.WriteLine($"Successfully deleted client '{clientId}'");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private static async Task ListClients()
    {
        var clients = _oAuthClientManager.GetAll();

        Console.WriteLine("The following clients are configured:");

        await foreach (var client in clients)
        {
            Console.WriteLine(JsonSerializer.Serialize(client, _jsonSerializerOptions));
        }
    }

    private static Task Exit()
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}

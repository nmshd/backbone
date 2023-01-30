using System.Reflection;
using System.Text.Json;
using Devices.Domain.Entities;
using Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Client;

namespace Devices.AdminCli;

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

    public static int Main(string[] args)
    {
        ApplicationConfiguration configuration;
        try
        {
            configuration = GetConfiguration(args);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return 1;
        }

        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services, configuration);

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        _oAuthClientManager = serviceProvider.GetRequiredService<OAuthClientManager>();

        Run();

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
                sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
            });

            // Register the entity sets needed by OpenIddict.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
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
            })

            // Register the OpenIddict client components.
            .AddClient(options =>
            {
                // Allow grant_type=password to be negotiated.
                options.AllowPasswordFlow();

                // Disable token storage, which is not necessary for non-interactive flows like
                // grant_type=password, grant_type=client_credentials or grant_type=refresh_token.
                options.DisableTokenStorage();

                // Register the System.Net.Http integration and use the identity of the current
                // assembly as a more specific user agent, which can be useful when dealing with
                // providers that use the user agent as a way to throttle requests (e.g Reddit).
                options.UseSystemNetHttp()
                       .SetProductInformation(typeof(Program).Assembly);

                // Add a client registration without a client identifier/secret attached.
                options.AddRegistration(new OpenIddictClientRegistration
                {
                    Issuer = new Uri("https://localhost:5000/", UriKind.Absolute)
                });
            });

        services.AddLogging();
        services.AddTransient<OAuthClientManager>();
    }

    private static ApplicationConfiguration GetConfiguration(string[] args)
    {
        IConfigurationRoot commandLineOptions = new ConfigurationBuilder().AddCommandLine(args, new Dictionary<string, string> { { "-c", "ConfigurationFile" } }).Build();
        string configurationFile = commandLineOptions.GetValue<string>("ConfigurationFile");

        IConfigurationBuilder configurationBuilder =
            new ConfigurationBuilder()
                .AddEnvironmentVariables();

        if (!string.IsNullOrEmpty(configurationFile))
        {
            string fullPathToConfigurationFile = Path.Combine(Environment.CurrentDirectory, configurationFile);
            configurationBuilder = configurationBuilder.AddJsonFile(fullPathToConfigurationFile, true, false);
        }

        configurationBuilder = configurationBuilder.AddCommandLine(args);

        IConfigurationRoot configuration = configurationBuilder.Build();

        ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration
        {
            DbConnectionString = configuration.GetValue<string>("Database:ConnectionString")
        };

        applicationConfiguration.Validate();

        Console.WriteLine("The following configuration is used: ");
        Console.WriteLine(JsonSerializer.Serialize(applicationConfiguration, _jsonSerializerOptions));

        return applicationConfiguration;
    }

    private static void Run()
    {
        while (true)
        {
            MenuItem userChoice = Menu.AskForItemChoice();
            userChoice.Action.Invoke();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static async void CreateClient()
    {
        string? clientId = ConsoleHelpers.ReadOptional("clientId (optional)");
        string? clientName = ConsoleHelpers.ReadOptional("displayName (optional)");
        string? clientSecret = ConsoleHelpers.ReadOptional("clientSecret (optional)");
        int? accessTokenLifetime = ConsoleHelpers.ReadOptionalNumber("accessTokenLifetime (default: 300)", 60, 3_600);

        CreatedClientDTO createdClient = await _oAuthClientManager.Create(clientId, clientName, clientSecret, accessTokenLifetime);

        Console.WriteLine(JsonSerializer.Serialize(createdClient, _jsonSerializerOptions));
        Console.WriteLine("Please note the secret since you cannot obtain it later.");
    }

    private static async void CreateAnonymousClient()
    {
        CreatedClientDTO createdClient = await _oAuthClientManager.Create(null, null, null, null);

        Console.WriteLine(JsonSerializer.Serialize(createdClient, _jsonSerializerOptions));
        Console.WriteLine("Please note the secret since you cannot obtain it later.");
    }

    private static void DeleteClient()
    {
        try
        {
            string clientId = ConsoleHelpers.ReadRequired("clientId");
            _oAuthClientManager.Delete(clientId);
            Console.WriteLine($"Successfully deleted client '{clientId}'");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private static async void ListClients()
    {
        IAsyncEnumerable<ClientDTO> clients = _oAuthClientManager.GetAll();

        Console.WriteLine("The following clients are configured:");

        await foreach (ClientDTO client in clients)
        {
            Console.WriteLine(JsonSerializer.Serialize(client, _jsonSerializerOptions));
        }
    }

    private static void Exit()
    {
        Environment.Exit(0);
    }
}

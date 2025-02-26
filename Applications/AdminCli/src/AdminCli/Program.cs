using System.CommandLine;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backbone.AdminCli.Configuration;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Announcements.Application.Extensions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RootCommand = Backbone.AdminCli.Commands.RootCommand;

namespace Backbone.AdminCli;

public class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: false)
            .Build();

        var serviceProvider = BuildServiceProvider(configuration);

        var rootCommand = serviceProvider.GetRequiredService<RootCommand>();

        await rootCommand.InvokeAsync(args);
    }

    private static IServiceProvider BuildServiceProvider(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        services.AddAutofac();

        services.AddSingleton(configuration);

        var commands = typeof(Program)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Command)) && !t.IsAbstract);

        foreach (var command in commands)
        {
            services.AddTransient(command);
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
                    .UseDbContext<DevicesDbContext>()
                    .ReplaceDefaultEntities<CustomOpenIddictEntityFrameworkCoreApplication, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreScope,
                        CustomOpenIddictEntityFrameworkCoreToken, string>();
                options.AddApplicationStore<CustomOpenIddictEntityFrameworkCoreApplicationStore>();
            });

        services.AddApplication();

        services.AddSingleton<IQuotaChecker, AlwaysSuccessQuotaChecker>();
        services.AddLogging();

        services.AddDevicesModule(configuration);
        services.AddAnnouncementsModule(configuration);

        services.ConfigureAndValidate<AdminCliConfiguration>(configuration.Bind);

        var serviceProvider = services.BuildServiceProvider();
#pragma warning disable ASP0000 // We retrieve the Configuration via IOptions here so that it is validated
        var parsedConfiguration = serviceProvider.GetRequiredService<IOptions<AdminCliConfiguration>>().Value;
#pragma warning restore ASP0000

        services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);
        services.AddPushNotifications(parsedConfiguration.Modules.Devices.Infrastructure.PushNotifications);

        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        return new AutofacServiceProvider(containerBuilder.Build());
    }
}

public static class IServiceCollectionExtensions
{
    public static void AddDevicesModule(this IServiceCollection services, IConfiguration configuration)
    {
        Modules.Devices.Application.Extensions.IServiceCollectionExtensions.AddApplication(services, configuration.GetSection("Modules:Devices:Application"));
        Modules.Devices.Infrastructure.Persistence.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = configuration["Modules:Devices:Infrastructure:SqlDatabase:Provider"]!;
            options.ConnectionString = configuration["Modules:Devices:Infrastructure:SqlDatabase:ConnectionString"]!;
        });
    }

    public static void AddAnnouncementsModule(this IServiceCollection services, IConfiguration configuration)
    {
        Modules.Announcements.Application.Extensions.IServiceCollectionExtensions.AddApplication(services);
        Modules.Announcements.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = configuration["Modules:Devices:Infrastructure:SqlDatabase:Provider"]!;
            options.ConnectionString = configuration["Modules:Devices:Infrastructure:SqlDatabase:ConnectionString"]!;
        });
    }
}

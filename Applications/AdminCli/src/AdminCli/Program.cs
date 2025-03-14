using System.CommandLine;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backbone.AdminCli.Configuration;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddSingleton(configuration);
        services.ConfigureAndValidate<AdminCliConfiguration>(configuration.Bind);

        services.AddAutofac();
        services.AddLogging();

        services.AddCliCommands();

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

        services.AddSingleton<IQuotaChecker, AlwaysSuccessQuotaChecker>();

        services.AddCommonInfrastructure();

        services
            .AddAnnouncementsModule(configuration)
            .AddDevicesModule(configuration)
            .AddTokensModule(configuration);

        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        return new AutofacServiceProvider(containerBuilder.Build());
    }
}

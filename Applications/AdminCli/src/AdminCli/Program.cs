using System.CommandLine;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backbone.AdminCli.Configuration;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.Modules.Announcements.Module;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Module;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Module;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InfrastructureConfiguration = Backbone.Modules.Devices.Infrastructure.InfrastructureConfiguration;
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
            .AddModule<AnnouncementsModule, Modules.Announcements.Application.ApplicationConfiguration, Modules.Announcements.Infrastructure.InfrastructureConfiguration>(configuration)
            .AddModule<DevicesModule, Modules.Devices.Application.ApplicationConfiguration, InfrastructureConfiguration>(configuration)
            .AddModule<TokensModule, ApplicationConfiguration, Modules.Tokens.Infrastructure.InfrastructureConfiguration>(configuration);

        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        return new AutofacServiceProvider(containerBuilder.Build());
    }
}

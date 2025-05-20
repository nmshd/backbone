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
using Backbone.Modules.Files.Module;
using Backbone.Modules.Messages.Module;
using Backbone.Modules.Relationships.Module;
using Backbone.Modules.Synchronization.Module;
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
    public const string METER_NAME = "enmeshed.backbone.admincli";

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
            .AddModule<FilesModule, Modules.Files.Application.ApplicationConfiguration, Modules.Files.Infrastructure.InfrastructureConfiguration>(configuration)
            .AddModule<MessagesModule, Modules.Messages.Application.ApplicationConfiguration, Modules.Messages.Infrastructure.InfrastructureConfiguration>(configuration)
            .AddModule<RelationshipsModule, Modules.Relationships.Application.ApplicationConfiguration, Modules.Relationships.Infrastructure.InfrastructureConfiguration>(configuration)
            .AddModule<SynchronizationModule, Modules.Synchronization.Application.ApplicationConfiguration, Modules.Synchronization.Infrastructure.InfrastructureConfiguration>(configuration)
            .AddModule<TokensModule, ApplicationConfiguration, Modules.Tokens.Infrastructure.InfrastructureConfiguration>(configuration);

        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        return new AutofacServiceProvider(containerBuilder.Build());
    }
}

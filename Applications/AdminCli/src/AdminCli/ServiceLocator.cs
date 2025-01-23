using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backbone.AdminCli.Configuration;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IServiceCollectionExtensions = Backbone.Modules.Devices.Infrastructure.Persistence.IServiceCollectionExtensions;


namespace Backbone.AdminCli;

public class ServiceLocator
{
    public T GetService<T>(string dbProvider, string dbConnectionString) where T : notnull
    {
        var services = ConfigureServices(dbProvider, dbConnectionString);
        return services.GetRequiredService<T>();
    }

    private static IServiceProvider ConfigureServices(string dbProvider, string dbConnectionString)
    {
        var services = new ServiceCollection();

        services.AddAutofac();

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

        services.AddApplicationWithoutIdentityDeletion();

        services.AddSingleton<IQuotaChecker, AlwaysSuccessQuotaChecker>();
        services.AddLogging();
        IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = dbProvider;
            options.ConnectionString = dbConnectionString;
        });

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.ConfigureAndValidate<AdminCliConfiguration>(configuration.Bind);

        var serviceProvider = services.BuildServiceProvider();
#pragma warning disable ASP0000 // We retrieve the Configuration via IOptions here so that it is validated
        var parsedConfiguration = serviceProvider.GetRequiredService<IOptions<AdminCliConfiguration>>().Value;
#pragma warning restore ASP0000

        services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);
        services.AddPushNotifications(parsedConfiguration.Modules.Devices.Infrastructure.PushNotifications);

        Modules.Announcements.Application.Extensions.IServiceCollectionExtensions.AddApplication(services);
        services.AddDatabase(options =>
        {
            options.Provider = dbProvider;
            options.ConnectionString = dbConnectionString;
        });

        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        var container = containerBuilder.Build();
        return new AutofacServiceProvider(container);
    }
}

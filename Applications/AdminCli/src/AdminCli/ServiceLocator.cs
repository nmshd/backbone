using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.AdminCli;

public class ServiceLocator
{
    public T GetService<T>(string dbProvider, string dbConnectionString) where T : notnull
    {
        var services = ConfigureServices(dbProvider, dbConnectionString);

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<T>();
    }

    private static IServiceCollection ConfigureServices(string dbProvider, string dbConnectionString)
    {
        var services = new ServiceCollection();
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
        services.AddDatabase(options =>
        {
            options.Provider = dbProvider;
            options.ConnectionString = dbConnectionString;
        });

        return services;
    }
}

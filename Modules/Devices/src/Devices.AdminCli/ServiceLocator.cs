using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.Devices.Application.Extensions;
using Backbone.Devices.Domain.Entities;
using Backbone.Devices.Infrastructure.OpenIddict;
using Backbone.Devices.Infrastructure.Persistence;
using Backbone.Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Devices.AdminCli;

public class ServiceLocator
{
    public T GetService<T>(string dbProvider, string dbConnectionString) where T : notnull
    {
        var services = ConfigureServices(dbProvider, dbConnectionString);

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<T>();
    }

    private IServiceCollection ConfigureServices(string dbProvider, string dbConnectionString)
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

        services.AddApplication();

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

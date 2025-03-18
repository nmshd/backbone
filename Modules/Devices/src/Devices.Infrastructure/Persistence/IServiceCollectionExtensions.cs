using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration configuration)
    {
        services.AddDbContextForModule<DevicesDbContext>(configuration, "Devices", setupDbContextOptions: dbContextOptions =>
        {
            dbContextOptions.UseOpenIddict<
                CustomOpenIddictEntityFrameworkCoreApplication,
                CustomOpenIddictEntityFrameworkCoreAuthorization,
                CustomOpenIddictEntityFrameworkCoreScope,
                CustomOpenIddictEntityFrameworkCoreToken,
                string>();
        });

        services.AddScoped<IDevicesDbContext, DevicesDbContext>();

        services.AddRepositories();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IIdentitiesRepository, IdentitiesRepository>();
        services.AddTransient<ITiersRepository, TiersRepository>();
        services.AddTransient<IChallengesRepository, ChallengesRepository>();
        services.AddTransient<IOAuthClientsRepository, OAuthClientsRepository>();
        services.AddTransient<IPnsRegistrationsRepository, PnsRegistrationsRepository>();
    }
}

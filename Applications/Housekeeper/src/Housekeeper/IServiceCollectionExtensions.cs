using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;

namespace Backbone.Housekeeper;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCustomOpenIddict(this IServiceCollection services)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<DevicesDbContext>()
                    .ReplaceDefaultEntities<
                        CustomOpenIddictEntityFrameworkCoreApplication,
                        CustomOpenIddictEntityFrameworkCoreAuthorization,
                        CustomOpenIddictEntityFrameworkCoreScope,
                        CustomOpenIddictEntityFrameworkCoreToken,
                        string
                    >();
            });

        return services;
    }
}

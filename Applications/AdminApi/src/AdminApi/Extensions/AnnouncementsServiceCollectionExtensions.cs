using Backbone.AdminApi.Configuration;
using Backbone.Modules.Announcements.Application.Extensions;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;

namespace Backbone.AdminApi.Extensions;

public static class AnnouncementsServiceCollectionExtensions
{
    public static IServiceCollection AddAnnouncements(this IServiceCollection services,
        AnnouncementsConfiguration configuration)
    {
        services.AddApplication();

        services.AddDatabase(options =>
        {
            options.ConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;
            options.Provider = configuration.Infrastructure.SqlDatabase.Provider;
        });

        return services;
    }
}

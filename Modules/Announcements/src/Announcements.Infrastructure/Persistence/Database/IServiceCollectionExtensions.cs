using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration configuration)
    {
        services.AddDbContextForModule<AnnouncementsDbContext>(configuration, "Announcements");

        services.AddTransient<IAnnouncementsRepository, AnnouncementsRepository>();
    }
}

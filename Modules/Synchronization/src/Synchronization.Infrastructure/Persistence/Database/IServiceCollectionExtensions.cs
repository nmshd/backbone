using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration options)
    {
        services.AddDbContextForModule<SynchronizationDbContext>(options, "Synchronization");

        services.AddScoped<ISynchronizationDbContext, SynchronizationDbContext>();
    }
}

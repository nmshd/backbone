using Backbone.Modules.Synchronization.Application.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Repository;

public static class IServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IRelationshipsRepository, RelationshipsRepository>();
    }
}

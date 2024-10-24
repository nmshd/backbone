using Backbone.Modules.Tags.Application.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tags.Infrastructure.Persistence.Repository;

public static class IServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ITagsRepository, TagsRepository>();
    }
}

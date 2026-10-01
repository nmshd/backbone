using Backbone.Modules.Tags.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tags.Infrastructure.Persistence.Repository;

public static class IServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ITagsRepository, TagsRepository>();
    }
}

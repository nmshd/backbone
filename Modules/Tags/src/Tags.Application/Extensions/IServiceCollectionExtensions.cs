using Backbone.Modules.Tags.Application.Tags.Queries.ListTags;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tags.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<ListTagsQuery>()
        );
    }
}

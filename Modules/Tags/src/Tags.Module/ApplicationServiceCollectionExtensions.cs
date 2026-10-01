using Backbone.Modules.Tags.Module.Features.Tags.ListTags;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tags.Module;

internal static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<ListTagsQuery>()
        );
    }
}

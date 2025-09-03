using Backbone.BuildingBlocks.Application.Abstractions.Housekeeping;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddHousekeeper<THousekeeper>(this IServiceCollection services) where THousekeeper : class, IHousekeeper
    {
        services.AddTransient<IHousekeeper, THousekeeper>();
    }
}

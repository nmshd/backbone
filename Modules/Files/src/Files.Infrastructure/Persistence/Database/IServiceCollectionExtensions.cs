using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration options)
    {
        services.AddDbContextForModule<FilesDbContext>(options, "Files");

        services.AddScoped<IFilesDbContext, FilesDbContext>();
    }
}

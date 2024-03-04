using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public static class DbContextOptionsBuilderExtensions
{
    public static void AddSaveChangesTimeInterceptor(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
    {
        builder.AddInterceptors(serviceProvider.GetRequiredService<SaveChangesTimeInterceptor>());
    }
}

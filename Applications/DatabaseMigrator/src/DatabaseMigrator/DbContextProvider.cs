using Microsoft.EntityFrameworkCore;

namespace Backbone.DatabaseMigrator;

public class DbContextProvider
{
    private readonly IServiceProvider _serviceProvider;

    public DbContextProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public DbContext GetDbContext(Type type)
    {
        var scope = _serviceProvider.CreateScope();
        var context = (DbContext)scope.ServiceProvider.GetRequiredService(type);
        return context;
    }
}

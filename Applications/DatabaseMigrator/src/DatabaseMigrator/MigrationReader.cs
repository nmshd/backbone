using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.DatabaseMigrator;

public class MigrationReader
{
    private readonly DbContextProvider _dbContextProvider;

    // CAUTION: the order of the db contexts in this list is important. The init migrations will be applied in the order of this list.
    private readonly List<Type> _dbContextTypes =
    [
        typeof(ChallengesDbContext),
        typeof(DevicesDbContext),
        typeof(FilesDbContext),
        typeof(RelationshipsDbContext),
        typeof(SynchronizationDbContext),
        typeof(TokensDbContext),
        typeof(MessagesDbContext),
        typeof(QuotasDbContext),
        typeof(AdminApiDbContext),
        typeof(AnnouncementsDbContext)
    ];

    public MigrationReader(DbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public async Task<IEnumerable<Migration>> ReadMigrations()
    {
        var migrations = await ReadAllFromDisk();
        return Order(migrations);
    }

    private async Task<List<Migration>> ReadAllFromDisk()
    {
        List<Migration> migrations = [];

        foreach (var dbContextType in _dbContextTypes)
        {
            var dbContext = _dbContextProvider.GetDbContext(dbContextType);
            var migrationNames = await dbContext.Database.GetPendingMigrationsAsync();

            migrations.AddRange(migrationNames.Select(n => new Migration(dbContextType, n)));
        }

        return migrations;
    }

    private IEnumerable<Migration> Order(List<Migration> migrations)
    {
        // Init migrations cannot be ordered by name, because they were created in an incorrect order.  
        // That's why we have to manually order them.
        var initMigrations = migrations.ExtractInitMigrations();

        return initMigrations
            .OrderBy(m => _dbContextTypes.IndexOf(m.DbContextType))
            .Concat(migrations
                .Except(initMigrations)
                .OrderBy(m => m.Name)
            );
    }
}

file static class Extensions
{
    public static List<Migration> ExtractInitMigrations(this List<Migration> migrations)
    {
        return migrations
            .FindAll(m => m.Name.EndsWith("Init"))
            .ToList();
    }
}

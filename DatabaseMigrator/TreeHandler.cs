using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;

namespace Backbone.DatabaseMigrator;

public class TreeHandler
{
    private List<MigrationInfo> _rawMigrations;

    public TreeHandler(List<MigrationInfo> rawMigrations)
    {
        _rawMigrations = rawMigrations;
    }
}

public record MigrationInfo(ModuleType Type, string Name, bool IsApplied, IList<MigrationDependency> Dependencies);

public record MigrationDependency(ModuleType Type, string Id);

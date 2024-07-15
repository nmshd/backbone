using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;

namespace Backbone.DatabaseMigrator;

public class TreeHandler
{
    private readonly List<MigrationInfo> _rawMigrations;

    public TreeHandler(List<MigrationInfo> rawMigrations)
    {
        _rawMigrations = rawMigrations;
        Console.WriteLine("Migrations:");
        foreach (var info in rawMigrations) Console.WriteLine($"{info.Type}: {info.Name}, {info.IsApplied}, {info.Dependencies.Count} dependencies");
    }
}

public record MigrationInfo(ModuleType Type, string Name, bool IsApplied, IList<MigrationDependency> Dependencies);

public record MigrationDependency(ModuleType Type, string Id);

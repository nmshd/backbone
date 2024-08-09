using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;

namespace Backbone.DatabaseMigrator;

public class GraphHandler
{
    private readonly List<List<MigrationInfo>> _graph = [[]];

    public IReadOnlyList<MigrationId> MigrationSequence => _graph
        .SelectMany(layer => layer)
        .Where(info => !info.IsApplied)
        .Select(info => info.Id)
        .ToList();

    public GraphHandler(List<MigrationInfo> rawMigrations)
    {
        if (!rawMigrations.Any(info => info.Dependencies.Count == 0))
            throw new ArgumentException("At least one migration must have 0 dependencies (otherwise we have circular dependencies)");

        //Construct the graph with worst case performance of O(n^2)
        List<MigrationInfo> migrations = [.. rawMigrations];
        List<MigrationInfo> lastSnapshot = [];
        Dictionary<MigrationId, int> tree = new();
        var highestLayer = 0;
        var i = 0;

        while (migrations.Count != 0)
        {
            //Check for circular dependency by making a snapshot of the remaining migrations and comparing at the beginning of every roundtrip
            if (i == 0)
            {
                if (migrations.SequenceEqual(lastSnapshot))
                {
                    var migrationsInCircularDependency = migrations
                        .Select(m => $"\t{m.Id.Type}: {m.Id.Id}\n")
                        .Aggregate(string.Concat);

                    throw new ArgumentException($"These migrations form a circular dependency:\n{migrationsInCircularDependency}");
                }

                lastSnapshot = [.. migrations];
            }

            var info = migrations[i];
            if (info.Dependencies.Count == 0)
            {
                _graph[0].Add(info);
                tree[info.Id] = 0;
                migrations.RemoveAt(i);
            }
            else if (info.Dependencies.All(tree.ContainsKey)) // All dependencies are already in the graph
            {
                var targetLayer = info.Dependencies
                    .Select(id => tree[id])
                    .Max() + 1;

                if (targetLayer > highestLayer)
                {
                    _graph.Add([]);
                    highestLayer++;
                }

                _graph[targetLayer].Add(info);
                tree[info.Id] = targetLayer;
                migrations.RemoveAt(i);
            }
            else
            {
                i++;
                i %= migrations.Count;
            }
        }
    }
}

public record MigrationInfo(MigrationId Id, bool IsApplied, IList<MigrationId> Dependencies);

public record MigrationId(ModuleType Type, string Id);

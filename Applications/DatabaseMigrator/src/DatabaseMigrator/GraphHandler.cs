using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;

namespace Backbone.DatabaseMigrator;

public class GraphHandler
{
    private readonly List<List<MigrationInfo>> _graph = [[]];

    public IReadOnlyList<MigrationInfo> MigrationSequence => _graph
        .SelectMany(layer => layer)
        .Where(info => !info.IsApplied)
        .ToList();

    public GraphHandler(List<MigrationInfo> rawMigrations)
    {
        if (!rawMigrations.Any(info => info.Dependencies.Count == 0))
            throw new ArgumentException("At least one migration must have 0 dependencies (otherwise we have circular dependencies)");

        //Construct the graph with worst case performance of O(n^2)
        Dictionary<MigrationId, int> tree = new();
        var highestLayer = 0;
        var i = 0;
        while (rawMigrations.Count != 0)
        {
            var info = rawMigrations[i];
            if (info.Dependencies.Count == 0)
            {
                _graph[0].Add(info);
                tree[info.Id] = 0;
                rawMigrations.RemoveAt(i);
            }
            else if (info.Dependencies.All(id => tree.ContainsKey(id))) // All dependencies are already in the graph
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
                rawMigrations.RemoveAt(i);
            }
            else
            {
                i++;
                i %= rawMigrations.Count;
            }
        }

        //Test
        //Console.WriteLine("Migrations:");
        //foreach (var info in rawMigrations) Console.WriteLine($"{info.Id.Type}: {info.Id.Id}, {info.IsApplied}, {info.Dependencies.Count} dependencies");

        Console.WriteLine();
        Console.WriteLine("Graph layers:");
        foreach (var layer in _graph)
        {
            Console.WriteLine(_graph.IndexOf(layer));
            foreach (var info in layer) Console.WriteLine($"\t{info.Id.Type}: {info.Id.Id}");
        }
    }
}

public record MigrationInfo(MigrationId Id, bool IsApplied, IList<MigrationId> Dependencies);

public record MigrationId(ModuleType Type, string Id);

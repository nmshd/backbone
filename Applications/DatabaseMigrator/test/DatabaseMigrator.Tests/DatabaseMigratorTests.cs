using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;

namespace Backbone.DatabaseMigrator.Tests;

public class DatabaseMigratorTests : AbstractTestsBase
{
    [Fact]
    public void A_list_without_dependencies_gives_back_the_same_list()
    {
        var infos = CreateMigrationsWithoutDependencies(20);
        var func = CreateHandlerFunc(infos);
        var handler = func.Should().NotThrow().Which;

        handler.MigrationSequence.Should().BeEquivalentTo(infos.Select(info => info.Id));
    }

    [Fact]
    public void A_circular_dependency_without_normal_migrations_triggers_the_first_error_check()
    {
        var infos = CreateCircularDependencyWithoutNormalMigrations(20);
        var func = CreateHandlerFunc(infos);
        func.Should().Throw<ArgumentException>().WithMessage("At least one migration must have 0 dependencies (otherwise we have circular dependencies)");
    }

    [Fact]
    public void A_circular_dependency_with_normal_migrations_triggers_the_loop_error_check()
    {
        var infos = CreateCircularDependencyWithNormalMigrations(20);
        var func = CreateHandlerFunc(infos);
        func.Should().Throw<ArgumentException>().WithMessage("These migrations form a circular dependency:*");
    }

    [Fact]
    public void A_regular_list_gives_a_valid_graph()
    {
        List<MigrationInfo> migrations =
        [
            new SimpleMigrationInfo("A", []), new SimpleMigrationInfo("B", []),
            new SimpleMigrationInfo("C", ["A"]), new SimpleMigrationInfo("D", []),
            new SimpleMigrationInfo("E", ["A", "C"]), new SimpleMigrationInfo("F", ["C", "E"])
        ];
        List<SimpleMigrationId> expectedIds = ["A", "B", "D", "C", "E", "F"];

        var func = CreateHandlerFunc(migrations);
        var handler = func.Should().NotThrow().Which;

        handler.MigrationSequence.Should().BeEquivalentTo(expectedIds);
    }

    private static Func<GraphHandler> CreateHandlerFunc(List<MigrationInfo> infos) => () => new GraphHandler(infos);

    private static List<MigrationInfo> CreateMigrationsWithoutDependencies(int num) => Enumerable.Range(0, num)
        .Select(i => new SimpleMigrationInfo(i, []).ToMigrationInfo())
        .ToList();

    /**
     * Creates a list with only circular dependencies, so that the first check will trigger
     */
    private static List<MigrationInfo> CreateCircularDependencyWithoutNormalMigrations(int num)
    {
        List<MigrationInfo> ret = [new SimpleMigrationInfo(0, [num - 1])];

        for (var i = 1; i < num; i++)
            ret.Add(new SimpleMigrationInfo(i, [i - 1]));

        return ret;
    }

    /**
     * Creates a list with one "normal" migration (without dependencies) "A", so that the first check in the graph handler passes.
     * All other dependencies form a circular dependency to trigger the inner check
     */
    private static List<MigrationInfo> CreateCircularDependencyWithNormalMigrations(int num)
    {
        List<MigrationInfo> ret =
        [
            new SimpleMigrationInfo("A", []),
            new SimpleMigrationInfo(0, [num - 1])
        ];

        for (var i = 1; i < num; i++)
            ret.Add(new SimpleMigrationInfo(i, [i - 1]));

        return ret;
    }
}

public record SimpleMigrationInfo(SimpleMigrationId Id, IList<SimpleMigrationId> Dependencies)
{
    public MigrationInfo ToMigrationInfo() => new(Id, false, Dependencies.Select(id => (MigrationId)id).ToList());

    public static implicit operator MigrationInfo(SimpleMigrationInfo i) => i.ToMigrationInfo();
}

public record SimpleMigrationId(string Id)
{
    public static implicit operator SimpleMigrationId(string id) => new(id);
    public static implicit operator SimpleMigrationId(int id) => new(id.ToString());
    public static implicit operator MigrationId(SimpleMigrationId i) => new(ModuleType.Challenges, i.Id);
}

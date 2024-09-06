using System.CommandLine;

namespace Backbone.PerformanceSnapshotCreator;

public class MainCommand : RootCommand
{
    public MainCommand()
    {
        AddCommand(new GeneratePoolsCommand());
        AddCommand(new CreateEntitiesCommand());
    }
}

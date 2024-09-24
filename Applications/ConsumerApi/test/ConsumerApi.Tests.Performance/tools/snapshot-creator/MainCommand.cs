using System.CommandLine;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator;

public class MainCommand : RootCommand
{
    public MainCommand()
    {
        AddCommand(new GeneratePoolsCommand());
        AddCommand(new CreateEntitiesCommand());
    }
}

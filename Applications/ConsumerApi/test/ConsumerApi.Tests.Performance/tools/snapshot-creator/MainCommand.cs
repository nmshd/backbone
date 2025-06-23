using System.CommandLine;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator;

public class MainCommand : RootCommand
{
    public MainCommand()
    {
        Subcommands.Add(new GeneratePoolsCommand());
        Subcommands.Add(new CreateEntitiesCommand());
    }
}

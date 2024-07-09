using System.CommandLine;

namespace Backbone.Identity.Pool.Creator;

public class MainCommand : RootCommand
{
    public MainCommand()
    {
        AddCommand(new GeneratePoolsCommand());
        AddCommand(new CreateEntitiesCommand());
    }
}

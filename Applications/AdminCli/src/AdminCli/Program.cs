using System.CommandLine;
using RootCommand = Backbone.AdminCli.Commands.RootCommand;

namespace Backbone.AdminCli;

public class Program
{
    private static async Task Main(string[] args)
    {
        await new RootCommand(new ServiceLocator()).InvokeAsync(args);
    }
}

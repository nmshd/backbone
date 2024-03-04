using System.CommandLine;
using RootCommand = Backbone.Modules.Devices.AdminCli.Commands.RootCommand;

namespace Backbone.Modules.Devices.AdminCli;

public class Program
{
    private static async Task Main(string[] args)
    {
        await new RootCommand(new ServiceLocator()).InvokeAsync(args);
    }
}

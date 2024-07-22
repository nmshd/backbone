using Backbone.Modules.Devices.AdminCli.Commands.Clients;
using Backbone.Modules.Devices.AdminCli.Commands.Tiers;

namespace Backbone.Modules.Devices.AdminCli.Commands;

public class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(ServiceLocator serviceLocator)
    {
        AddCommand(new ClientCommand(serviceLocator));
        AddCommand(new TierCommand(serviceLocator));
    }
}

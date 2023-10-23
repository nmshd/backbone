using Backbone.Devices.AdminCli.Commands.Clients;
using Backbone.Devices.AdminCli.Commands.Tiers;

namespace Backbone.Devices.AdminCli.Commands;

public class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(ServiceLocator serviceLocator)
    {
        AddCommand(new ClientCommand(serviceLocator));
        AddCommand(new TierCommand(serviceLocator));
    }
}

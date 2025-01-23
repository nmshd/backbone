using Backbone.AdminCli.Commands.Announcements;
using Backbone.AdminCli.Commands.Clients;
using Backbone.AdminCli.Commands.Tiers;

namespace Backbone.AdminCli.Commands;

public class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(ServiceLocator serviceLocator)
    {
        AddCommand(new ClientCommand(serviceLocator));
        AddCommand(new TierCommand(serviceLocator));
        AddCommand(new AnnouncementCommand(serviceLocator));
    }
}

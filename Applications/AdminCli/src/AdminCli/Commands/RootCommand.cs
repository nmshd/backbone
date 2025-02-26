using Backbone.AdminCli.Commands.Announcements;
using Backbone.AdminCli.Commands.Clients;
using Backbone.AdminCli.Commands.Tiers;

namespace Backbone.AdminCli.Commands;

public class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(ClientCommand clientCommand, TierCommand tierCommand, AnnouncementCommand announcementCommand)
    {
        AddCommand(clientCommand);
        AddCommand(tierCommand);
        AddCommand(announcementCommand);
    }
}

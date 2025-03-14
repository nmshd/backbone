using Backbone.AdminCli.Commands.Announcements;
using Backbone.AdminCli.Commands.Clients;
using Backbone.AdminCli.Commands.Tiers;
using Backbone.AdminCli.Commands.Tokens;

namespace Backbone.AdminCli.Commands;

public class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(ClientCommand clientCommand, TierCommand tierCommand, AnnouncementCommand announcementCommand, TokenCommand tokenCommand)
    {
        AddCommand(clientCommand);
        AddCommand(tierCommand);
        AddCommand(announcementCommand);
        AddCommand(tokenCommand);
    }
}

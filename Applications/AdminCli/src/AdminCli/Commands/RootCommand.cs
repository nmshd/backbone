using Backbone.AdminCli.Commands.Announcements;
using Backbone.AdminCli.Commands.Clients;
using Backbone.AdminCli.Commands.Database;
using Backbone.AdminCli.Commands.Tiers;
using Backbone.AdminCli.Commands.Tokens;

namespace Backbone.AdminCli.Commands;

public class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(ClientCommand clientCommand, TierCommand tierCommand, AnnouncementCommand announcementCommand, TokenCommand tokenCommand, DatabaseCommand databaseCommand)
    {
        Subcommands.Add(clientCommand);
        Subcommands.Add(databaseCommand);
        Subcommands.Add(tierCommand);
        Subcommands.Add(announcementCommand);
        Subcommands.Add(tokenCommand);
    }
}

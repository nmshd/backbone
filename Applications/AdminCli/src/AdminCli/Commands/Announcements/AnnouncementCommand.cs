using System.CommandLine;

namespace Backbone.AdminCli.Commands.Announcements;

public class AnnouncementCommand : Command
{
    public AnnouncementCommand(SendAnnouncementCommand sendAnnouncementCommand,
        DeleteAnnouncementCommand deleteAnnouncementCommand) : base("announcement")
    {
        AddCommand(sendAnnouncementCommand);
        AddCommand(deleteAnnouncementCommand);
    }
}

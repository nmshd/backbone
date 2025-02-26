using System.CommandLine;

namespace Backbone.AdminCli.Commands.Announcements;

public class AnnouncementCommand : Command
{
    public AnnouncementCommand(SendAnnouncementCommand sendAnnouncementCommand) : base("announcement")
    {
        AddCommand(sendAnnouncementCommand);
    }
}

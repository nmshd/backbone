using Backbone.AdminCli.Commands.BaseClasses;

namespace Backbone.AdminCli.Commands.Announcements;

public class AnnouncementCommand : AdminCliCommand
{
    public AnnouncementCommand(ServiceLocator serviceLocator) : base("announcement", serviceLocator)
    {
        AddCommand(new SendAnnouncementCommand(serviceLocator));
    }
}

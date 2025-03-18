using System.CommandLine;
using Backbone.AdminCli.Commands.BaseClasses;
using Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementById;
using MediatR;

namespace Backbone.AdminCli.Commands.Announcements;

public class DeleteAnnouncementCommand : AdminCliCommand
{
    public DeleteAnnouncementCommand(IMediator mediator) : base(mediator, "delete", "Delete an announcement")
    {
        var announcementId = new Option<string>("--id")
        {
            IsRequired = true,
            Description = "The id of the Announcement that should be deleted."
        };

        AddOption(announcementId);

        this.SetHandler(DeleteAnnouncement, announcementId);
    }

    private async Task DeleteAnnouncement(string announcementId)
    {
        try
        {
            await _mediator.Send(new DeleteAnnouncementByIdCommand(announcementId), CancellationToken.None);
            Console.WriteLine($@"Successfully deleted announcement with id '{announcementId}'");
        }
        catch (Exception e)
        {
            Console.WriteLine($@"An error occurred: {e.Message}");
        }
    }
}

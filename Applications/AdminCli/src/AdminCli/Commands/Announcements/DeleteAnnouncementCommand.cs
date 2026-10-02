using DeleteAnnouncementById = Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementById;
using System.CommandLine;
using Backbone.AdminCli.Commands.BaseClasses;
using MediatR;

namespace Backbone.AdminCli.Commands.Announcements;

public class DeleteAnnouncementCommand : AdminCliCommand
{
    public DeleteAnnouncementCommand(IMediator mediator) : base(mediator, "delete", "Delete an announcement")
    {
        var announcementId = new Option<string>("--id")
        {
            Required = true,
            Description = "The id of the Announcement that should be deleted."
        };

        Options.Add(announcementId);

        SetAction((parseResult, token) =>
        {
            var announcementIdValue = parseResult.GetRequiredValue(announcementId);
            return DeleteAnnouncement(announcementIdValue);
        });
    }

    private async Task DeleteAnnouncement(string announcementId)
    {
        try
        {
            await _mediator.Send(new DeleteAnnouncementById.Command { Id = announcementId });
            Console.WriteLine($@"Successfully deleted announcement with id '{announcementId}'");
        }
        catch (Exception e)
        {
            Console.WriteLine($@"An error occurred: {e.Message}");
        }
    }
}

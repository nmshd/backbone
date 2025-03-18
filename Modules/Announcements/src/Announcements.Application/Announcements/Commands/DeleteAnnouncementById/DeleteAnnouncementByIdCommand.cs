using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementById;

public class DeleteAnnouncementByIdCommand(string id) : IRequest
{
    public string Id { get; } = id;
}

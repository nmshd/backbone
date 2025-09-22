using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementById;

public class DeleteAnnouncementByIdCommand : IRequest
{
    public required string Id { get; init; }
}

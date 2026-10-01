using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementById;

public class DeleteAnnouncementByIdCommand : IRequest
{
    public required string Id { get; init; }
}

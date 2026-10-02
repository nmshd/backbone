using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementById;

public class Command : IRequest
{
    public required string Id { get; init; }
}

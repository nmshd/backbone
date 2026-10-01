using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;
using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.GetAnnouncementById;

public class GetAnnouncementByIdQuery : IRequest<AnnouncementDTO>
{
    public required string Id { get; init; }
}

using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAnnouncementById;

public class GetAnnouncementByIdQuery : IRequest<AnnouncementDTO>
{
    public GetAnnouncementByIdQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.ListAnnouncements;

public class Handler : IRequestHandler<ListAnnouncementsQuery, ListAnnouncementsResponse>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task<ListAnnouncementsResponse> Handle(ListAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var announcements = await _announcementsRepository.List(cancellationToken);

        return new ListAnnouncementsResponse(announcements);
    }
}

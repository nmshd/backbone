using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.ListAllAnnouncements;

public class Handler : IRequestHandler<ListAllAnnouncementsQuery, ListAllAnnouncementsResponse>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task<ListAllAnnouncementsResponse> Handle(ListAllAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var announcements = await _announcementsRepository.List(cancellationToken);

        return new ListAllAnnouncementsResponse(announcements);
    }
}

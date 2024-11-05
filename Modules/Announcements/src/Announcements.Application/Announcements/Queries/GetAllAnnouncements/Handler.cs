using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncements;

public class Handler : IRequestHandler<GetAllAnnouncementsQuery, GetAllAnnouncementsResponse>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task<GetAllAnnouncementsResponse> Handle(GetAllAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var announcements = await _announcementsRepository.FindAll(cancellationToken);

        return new GetAllAnnouncementsResponse(announcements);
    }
}

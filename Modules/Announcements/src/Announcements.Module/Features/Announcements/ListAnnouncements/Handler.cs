using Backbone.Modules.Announcements.Abstractions;
using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncements;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var announcements = await _announcementsRepository.List(cancellationToken);

        return new Response(announcements);
    }
}

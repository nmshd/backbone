using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;
using Backbone.Modules.Announcements.Abstractions;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.GetAnnouncementById;

public class Handler : IRequestHandler<Query, AnnouncementDTO>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task<AnnouncementDTO> Handle(Query request, CancellationToken cancellationToken)
    {
        var announcementId = AnnouncementId.Parse(request.Id);

        var announcements = await _announcementsRepository.Get(announcementId, cancellationToken) ?? throw new NotFoundException(nameof(Announcement));

        return new AnnouncementDTO(announcements);
    }
}

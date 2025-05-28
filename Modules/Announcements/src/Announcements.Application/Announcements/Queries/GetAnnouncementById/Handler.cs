using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAnnouncementById;

public class Handler : IRequestHandler<GetAnnouncementByIdQuery, AnnouncementDTO>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task<AnnouncementDTO> Handle(GetAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var announcementId = AnnouncementId.Parse(request.Id);

        var announcements = await _announcementsRepository.Get(announcementId, cancellationToken) ?? throw new NotFoundException(nameof(Announcement));

        return new AnnouncementDTO(announcements);
    }
}

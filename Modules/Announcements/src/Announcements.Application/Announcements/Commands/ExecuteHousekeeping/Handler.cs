using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteAnnouncements(cancellationToken);
    }

    private async Task DeleteAnnouncements(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("announcements", ct => _announcementsRepository.Delete(Announcement.CanBeCleanedUp, ct), cancellationToken);
    }
}

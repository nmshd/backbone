using Backbone.BuildingBlocks.Application.Abstractions.Housekeeping;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application;

public class Housekeeper : IHousekeeper
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Housekeeper(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task<HousekeepingResponse> Execute(CancellationToken cancellationToken)
    {
        var responseItem = await DeleteAnnouncements(cancellationToken);
        return new HousekeepingResponse { Items = [responseItem] };
    }

    private async Task<HousekeepingResponseItem> DeleteAnnouncements(CancellationToken cancellationToken)
    {
        var deletedAnnouncementsCount = await _announcementsRepository.Delete(Announcement.CanBeCleanedUp, cancellationToken);

        return new HousekeepingResponseItem { EntityType = typeof(Announcement), NumberOfDeletedEntities = deletedAnnouncementsCount };
    }
}

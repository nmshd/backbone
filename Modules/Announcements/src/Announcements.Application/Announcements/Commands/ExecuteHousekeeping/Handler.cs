using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IAnnouncementsRepository _announcementsRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IAnnouncementsRepository announcementsRepository, ILogger<Handler> logger)
    {
        _announcementsRepository = announcementsRepository;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteAnnouncements(cancellationToken);
    }

    private async Task DeleteAnnouncements(CancellationToken cancellationToken)
    {
        var numberOfDeletedItems = await _announcementsRepository.Delete(Announcement.CanBeCleanedUp, cancellationToken);

        _logger.DataDeleted(numberOfDeletedItems, "announcements");
    }
}

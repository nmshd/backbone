using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementById;

public class Handler : IRequestHandler<DeleteAnnouncementByIdCommand>
{
    private readonly IAnnouncementsRepository _announcementsRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IAnnouncementsRepository announcementsRepository, ILogger<Handler> logger)
    {
        _announcementsRepository = announcementsRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteAnnouncementByIdCommand request, CancellationToken cancellationToken)
    {
        var parsedId = AnnouncementId.Parse(request.Id);
        var linesDeleted = await _announcementsRepository.Delete(parsedId, cancellationToken);

        if (linesDeleted > 0)
            _logger.AnnouncementDeleted();
        else
            throw new NotFoundException(nameof(Announcement));
    }
}

internal static partial class DeleteAnnouncementByIdLogs
{
    [LoggerMessage(
        EventId = 430695,
        EventName = "Announcements.DeleteAnnouncement.AnnouncementDeleted",
        Level = LogLevel.Information,
        Message = "The announcement was deleted.")]
    public static partial void AnnouncementDeleted(this ILogger logger);
}

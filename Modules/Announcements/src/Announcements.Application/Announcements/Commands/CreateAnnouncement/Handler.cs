using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;

public class Handler : IRequestHandler<CreateAnnouncementCommand, AnnouncementDTO>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task<AnnouncementDTO> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var recipients = request.Recipients.Select(r => new AnnouncementRecipient(IdentityAddress.Parse(r)));
        var actions = request.Actions.Select(a => new AnnouncementAction(a.DisplayName.ToDictionary(kv => AnnouncementLanguage.Parse(kv.Key), kv => kv.Value), a.Link));
        var texts = request.Texts.Select(t => new AnnouncementText(AnnouncementLanguage.Parse(t.Language), t.Title, t.Body)).ToList();

        var iqlQuery = request.IqlQuery == null ? null : AnnouncementIqlQuery.Parse(request.IqlQuery);

        var announcement = new Announcement(request.Severity, request.IsSilent, texts, request.ExpiresAt, recipients, actions, iqlQuery);

        await _announcementsRepository.Add(announcement, cancellationToken);

        return new AnnouncementDTO(announcement);
    }
}

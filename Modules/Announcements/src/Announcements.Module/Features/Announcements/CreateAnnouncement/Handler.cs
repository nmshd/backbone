using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;
using Backbone.Modules.Announcements.Abstractions;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.CreateAnnouncement;

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
        var texts = request.Texts.Select(t => new AnnouncementText(AnnouncementLanguage.Parse(t.Language), t.Title, t.Body)).ToList();
        var actions = request.Actions.Select((a, i) => new AnnouncementAction(a.DisplayName.ToDictionary(kv => AnnouncementLanguage.Parse(kv.Key), kv => kv.Value), a.Link, (byte)i));

        var iqlQuery = string.IsNullOrEmpty(request.IqlQuery) ? null : AnnouncementIqlQuery.Parse(request.IqlQuery);

        var announcement = new Announcement(request.Severity, request.IsSilent, texts, request.ExpiresAt, recipients, actions, iqlQuery);

        await _announcementsRepository.Add(announcement, cancellationToken);

        return new AnnouncementDTO(announcement);
    }
}

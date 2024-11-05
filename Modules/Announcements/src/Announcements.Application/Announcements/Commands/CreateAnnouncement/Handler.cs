﻿using Backbone.Modules.Announcements.Application.Announcements.DTOs;
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
        var texts = request.Texts.Select(t => new AnnouncementText(AnnouncementLanguage.ParseUnsafe(t.Language), t.Title, t.Body)).ToList();

        var announcement = new Announcement(request.Severity, texts, request.ExpiresAt);

        await _announcementsRepository.Add(announcement, cancellationToken);

        return new AnnouncementDTO(announcement);
    }
}

using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;

public class Handler : IRequestHandler<CreateAnnouncementCommand, AnnouncementDTO>
{
    private readonly IAnnouncementsRepository _announcementsRepository;
    private readonly IIdentitiesRepository _identityRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(ILogger<Handler> logger, IAnnouncementsRepository announcementsRepository, IIdentitiesRepository identityRepository)
    {
        _announcementsRepository = announcementsRepository;
        _logger = logger;
        _identityRepository = identityRepository;
    }

    public async Task<AnnouncementDTO> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        List<AnnouncementRecipient> announcementRecipients = [];

        if (request.Recipients != null && request.Recipients.Count != 0)
        {
            var requestRecipients = request.Recipients.OrderBy(address => address).ToList();

            var requestRecipientsAsIdentityAddresses = requestRecipients.Select(IdentityAddress.Parse).ToList();
            var foundRecipients = (await _identityRepository.Find(Identity.HasAddress(requestRecipientsAsIdentityAddresses), cancellationToken)).ToArray();
            var foundRecipientAddresses = foundRecipients.Select(r => r.Address.Value).OrderBy(address => address).ToList();
            var notFoundRecipientAddresses = requestRecipients.Except(foundRecipientAddresses).ToList();

            if (notFoundRecipientAddresses.Count > 0)
            {
                _logger.LogError("Not all recipients were found in the database. \r\n" +
                                 "Request Recipients: {requestRecipients}. \r\n" +
                                 "Not Found Recipients: {notFoundRecipientAddresses}",
                    string.Join(',', requestRecipients),
                    string.Join(',', notFoundRecipientAddresses));
            }

            announcementRecipients.AddRange(foundRecipients.Select(recipient => new AnnouncementRecipient(recipient.Address)));
        }

        var texts = request.Texts.Select(t => new AnnouncementText(AnnouncementLanguage.Parse(t.Language), t.Title, t.Body)).ToList();

        var announcement = new Announcement(request.Severity, texts, request.ExpiresAt, announcementRecipients);

        await _announcementsRepository.Add(announcement, cancellationToken);

        return new AnnouncementDTO(announcement);
    }
}

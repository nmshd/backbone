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
    private readonly IAnnouncementRecipientsRepository _announcementRecipientRepository;
    private readonly IAnnouncementsRepository _announcementsRepository;
    private readonly IIdentitiesRepository _identityRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IAnnouncementsRepository announcementsRepository, ILogger<Handler> logger, IIdentitiesRepository identityRepository,
        IAnnouncementRecipientsRepository announcementRecipientRepository)
    {
        _announcementsRepository = announcementsRepository;
        _logger = logger;
        _identityRepository = identityRepository;
        _announcementRecipientRepository = announcementRecipientRepository;
    }

    public async Task<AnnouncementDTO> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        List<AnnouncementRecipient> announcementRecipients = [];

        if (request.Recipients != null && request.Recipients.Count != 0)
        {
            var requestRecipients = request.Recipients.OrderBy(address => address).ToList();

            var foundRecipients = (await _identityRepository.Find(Identity.ContainsAddressValue(requestRecipients), cancellationToken)).ToArray();
            var foundRecipientAddresses = foundRecipients.Select(r => r.Address.Value).OrderBy(address => address).ToList();
            var notFoundRecipientAddresses = requestRecipients.Except(foundRecipientAddresses).ToList();

            if (notFoundRecipientAddresses.Count != 0)
            {
                _logger.LogError("Not all recipients were found in the database. \r\n" +
                                 "Request Recipients: {requestRecipients}. \r\n" +
                                 "Not Found Recipients: {notFoundRecipientAddresses}",
                    string.Join(',', requestRecipients),
                    string.Join(',', notFoundRecipientAddresses));
            }

            foreach (var recipient in foundRecipients)
            {
                announcementRecipients.AddRange(recipient.Devices.Select(recipientDevice =>
                    new AnnouncementRecipient(recipientDevice.Id.Value, recipient.Address.Value)));
            }

            foreach (var announcementRecipient in announcementRecipients)
            {
                await _announcementRecipientRepository.Add(announcementRecipient, cancellationToken);
            }
        }

        var texts = request.Texts.Select(t => new AnnouncementText(AnnouncementLanguage.Parse(t.Language), t.Title, t.Body)).ToList();

        var announcement = new Announcement(request.Severity, texts, request.ExpiresAt, announcementRecipients);

        await _announcementsRepository.Add(announcement, cancellationToken);

        return new AnnouncementDTO(announcement);
    }
}

using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.AnonymizeRecipient;

public class Handler : IRequestHandler<AnonymizeRecipientForIdentityCommand>
{
    private readonly IAnnouncementRecipientRepository _announcementRecipientRepository;
    private readonly ApplicationOptions _applicationOptions;

    public Handler(IAnnouncementRecipientRepository announcementRecipientRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _announcementRecipientRepository = announcementRecipientRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(AnonymizeRecipientForIdentityCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdentityAddress)) return;

        var announcementRecipients = await _announcementRecipientRepository.FindAllForIdentityAddress(IdentityAddress.Parse(request.IdentityAddress), cancellationToken);

        foreach (var announcementRecipient in announcementRecipients)
        {
            announcementRecipient.Anonymize(_applicationOptions.DidDomainName);
        }

        await _announcementRecipientRepository.Update(announcementRecipients, cancellationToken);
    }
}

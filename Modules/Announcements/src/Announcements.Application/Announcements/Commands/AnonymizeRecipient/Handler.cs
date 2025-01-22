using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.AnonymizeRecipient;

public class Handler : IRequestHandler<AnonymizeRecipientForIdentityCommand>
{
    private readonly IAnnouncementsRepository _announcementsRepository;
    private readonly ApplicationOptions _applicationOptions;

    public Handler(IAnnouncementsRepository announcementsRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _announcementsRepository = announcementsRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(AnonymizeRecipientForIdentityCommand request, CancellationToken cancellationToken)
    {
        var parsedIdentityAddress = IdentityAddress.Parse(request.IdentityAddress);

        var announcements = await _announcementsRepository.FindAllForIdentityAddress(
            announcement => announcement.Recipients.Any(r => r.Address == parsedIdentityAddress), cancellationToken);

        foreach (var announcementRecipient in announcements.SelectMany(announcement => announcement.Recipients))
        {
            announcementRecipient.Anonymize(_applicationOptions.DidDomainName);
        }

        await _announcementsRepository.Update(announcements, cancellationToken);
    }
}

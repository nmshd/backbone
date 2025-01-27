using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementRecipients;

public class Handler : IRequestHandler<DeleteAnnouncementRecipientsCommand>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task Handle(DeleteAnnouncementRecipientsCommand request, CancellationToken cancellationToken)
    {
        var parsedIdentityAddress = IdentityAddress.Parse(request.IdentityAddress);

        await _announcementsRepository.DeleteRecipients(r => r.Address == parsedIdentityAddress, cancellationToken);
    }
}

using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncementsInLanguage;

public class Handler : IRequestHandler<GetAllAnnouncementsForActiveIdentityInLanguageQuery, GetAllAnnouncementsInLanguageResponse>
{
    private readonly IAnnouncementsRepository _announcementsRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IAnnouncementsRepository announcementsRepository, IUserContext userContext)
    {
        _announcementsRepository = announcementsRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<GetAllAnnouncementsInLanguageResponse> Handle(GetAllAnnouncementsForActiveIdentityInLanguageQuery request, CancellationToken cancellationToken)
    {
        var announcements = await _announcementsRepository.FindAll(Announcement.IsForRecipient(_activeIdentity), cancellationToken);

        var expectedLanguage = AnnouncementLanguage.Parse(request.Language);

        return new GetAllAnnouncementsInLanguageResponse(announcements, expectedLanguage);
    }
}

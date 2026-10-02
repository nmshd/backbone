using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Abstractions;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsInLanguage;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IAnnouncementsRepository _announcementsRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IAnnouncementsRepository announcementsRepository, IUserContext userContext)
    {
        _announcementsRepository = announcementsRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var announcements = await _announcementsRepository.List(Announcement.IsForRecipient(_activeIdentity), cancellationToken);

        var expectedLanguage = AnnouncementLanguage.Parse(request.Language);

        return new Response(announcements, expectedLanguage);
    }
}

using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncementsInLanguage;

public class Handler : IRequestHandler<GetAllAnnouncementsInLanguageQuery, GetAllAnnouncementsInLanguageResponse>
{
    private readonly IAnnouncementsRepository _announcementsRepository;

    public Handler(IAnnouncementsRepository announcementsRepository)
    {
        _announcementsRepository = announcementsRepository;
    }

    public async Task<GetAllAnnouncementsInLanguageResponse> Handle(GetAllAnnouncementsInLanguageQuery request, CancellationToken cancellationToken)
    {
        var announcements = await _announcementsRepository.FindAll(cancellationToken);

        var expectedLanguage = AnnouncementLanguage.Parse(request.Language);

        return new GetAllAnnouncementsInLanguageResponse(announcements, expectedLanguage);
    }
}

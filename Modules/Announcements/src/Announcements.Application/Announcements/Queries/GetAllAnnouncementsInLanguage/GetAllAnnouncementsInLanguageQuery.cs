using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncementsInLanguage;

public class GetAllAnnouncementsInLanguageQuery : IRequest<GetAllAnnouncementsInLanguageResponse>
{
    public required string Language { get; set; }
}

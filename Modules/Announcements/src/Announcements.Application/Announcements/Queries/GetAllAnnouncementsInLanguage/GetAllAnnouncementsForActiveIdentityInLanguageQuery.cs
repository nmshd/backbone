using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncementsInLanguage;

public class GetAllAnnouncementsForActiveIdentityInLanguageQuery : IRequest<GetAllAnnouncementsInLanguageResponse>
{
    public required string Language { get; set; }
}

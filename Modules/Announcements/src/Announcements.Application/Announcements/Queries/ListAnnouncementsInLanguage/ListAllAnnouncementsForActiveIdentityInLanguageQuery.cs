using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.ListAnnouncementsInLanguage;

public class ListAnnouncementsForActiveIdentityInLanguageQuery : IRequest<ListAnnouncementsInLanguageResponse>
{
    public required string Language { get; set; }
}

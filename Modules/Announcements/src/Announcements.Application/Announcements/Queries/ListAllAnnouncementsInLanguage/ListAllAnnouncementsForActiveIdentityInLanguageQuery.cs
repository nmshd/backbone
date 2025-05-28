using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.ListAllAnnouncementsInLanguage;

public class ListAllAnnouncementsForActiveIdentityInLanguageQuery : IRequest<ListAllAnnouncementsInLanguageResponse>
{
    public required string Language { get; set; }
}

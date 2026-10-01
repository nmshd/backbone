using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsInLanguage;

public class ListAnnouncementsForActiveIdentityInLanguageQuery : IRequest<ListAnnouncementsInLanguageResponse>
{
    public required string Language { get; init; }
}

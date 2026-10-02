using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsForActiveIdentityInLanguage;

public class Query : IRequest<Response>
{
    public required string Language { get; init; }
}

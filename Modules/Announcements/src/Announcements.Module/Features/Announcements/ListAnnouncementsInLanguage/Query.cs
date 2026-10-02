using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsInLanguage;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListAnnouncementsForActiveIdentityInLanguageQuery")]
public class Query : IRequest<Response>
{
    public required string Language { get; init; }
}

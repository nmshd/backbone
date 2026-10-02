using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;
using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.GetAnnouncementById;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetAnnouncementByIdQuery")]
public class Query : IRequest<AnnouncementDTO>
{
    public required string Id { get; init; }
}

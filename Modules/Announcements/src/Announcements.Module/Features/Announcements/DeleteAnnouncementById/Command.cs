using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementById;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteAnnouncementByIdCommand")]
public class Command : IRequest
{
    public required string Id { get; init; }
}

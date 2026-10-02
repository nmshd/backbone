using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncements;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListAnnouncementsQuery")]
public class Query : IRequest<Response>;

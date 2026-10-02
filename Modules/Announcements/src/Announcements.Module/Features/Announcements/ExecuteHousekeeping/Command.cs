using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ExecuteHousekeeping;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ExecuteHousekeepingCommand")]
public class Command : IRequest;

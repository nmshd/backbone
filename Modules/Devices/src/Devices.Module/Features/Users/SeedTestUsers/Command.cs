using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Users.SeedTestUsers;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("SeedTestUsersCommand")]
public class Command : IRequest;

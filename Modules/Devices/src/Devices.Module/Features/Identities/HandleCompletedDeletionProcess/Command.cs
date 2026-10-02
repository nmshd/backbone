using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.HandleCompletedDeletionProcess;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("HandleCompletedDeletionProcessCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
    public required IEnumerable<string> Usernames { get; init; }
}

using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.DeleteIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}

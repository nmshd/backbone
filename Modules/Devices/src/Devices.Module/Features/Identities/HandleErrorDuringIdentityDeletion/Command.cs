using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.HandleErrorDuringIdentityDeletion;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("HandleErrorDuringIdentityDeletionCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
    public required string ErrorMessage { get; init; }
}

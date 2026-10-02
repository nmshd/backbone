using MediatR;

namespace Backbone.Modules.Files.Module.Features.Identities.DeleteFilesOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteFilesOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}

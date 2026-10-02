using MediatR;

namespace Backbone.Modules.Files.Module.Features.Identities.AnonymizeCreatedByOfFiles;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("AnonymizeCreatedByOfFilesCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}

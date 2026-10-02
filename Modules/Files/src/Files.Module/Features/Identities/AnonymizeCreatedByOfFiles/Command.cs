using MediatR;

namespace Backbone.Modules.Files.Module.Features.Identities.AnonymizeCreatedByOfFiles;

public class AnonymizeCreatedByOfFilesCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}

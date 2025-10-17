using MediatR;

namespace Backbone.Modules.Files.Application.Identities.Commands.AnonymizeCreatedByOfFiles;

public class AnonymizeCreatedByOfFilesCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}

using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.RegenerateFileOwnershipToken;

public class RegenerateFileOwnershipTokenCommand : IRequest<string>
{
    public required string FileAddress { get; init; }
}

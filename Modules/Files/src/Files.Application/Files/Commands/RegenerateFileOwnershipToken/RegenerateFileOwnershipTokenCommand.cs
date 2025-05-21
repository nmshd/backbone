using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.RegenerateFileOwnershipToken;

public class RegenerateFileOwnershipTokenCommand : IRequest<RegenerateFileOwnershipTokenResponse>
{
    public required string FileId { get; init; }
}

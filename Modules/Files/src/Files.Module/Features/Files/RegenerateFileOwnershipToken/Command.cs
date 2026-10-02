using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.RegenerateFileOwnershipToken;

public class RegenerateFileOwnershipTokenCommand : IRequest<RegenerateFileOwnershipTokenResponse>
{
    public required string FileId { get; init; }
}

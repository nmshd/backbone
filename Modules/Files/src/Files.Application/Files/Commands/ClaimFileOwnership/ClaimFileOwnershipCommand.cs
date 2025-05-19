using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.ClaimFileOwnership;

public class ClaimFileOwnershipCommand(string ownershipToken, string fileId) : IRequest<ClaimFileOwnershipResponse>
{
    public required string FileId { get; init; } = fileId;
    public required string OwnershipToken { get; init; } = ownershipToken;
}

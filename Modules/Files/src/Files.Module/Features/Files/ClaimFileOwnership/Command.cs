using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.ClaimFileOwnership;

public class ClaimFileOwnershipCommand : IRequest<ClaimFileOwnershipResponse>
{
    public required string FileId { get; init; }
    public required string OwnershipToken { get; init; }
}

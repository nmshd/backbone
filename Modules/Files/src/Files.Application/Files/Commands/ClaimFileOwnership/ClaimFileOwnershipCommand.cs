using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.ClaimFileOwnership;

public class ClaimFileOwnershipCommand : IRequest<ClaimFileOwnershipResponse>
{
    public required string FileId { get; set; }
    public required string OwnershipToken { get; set; }
}

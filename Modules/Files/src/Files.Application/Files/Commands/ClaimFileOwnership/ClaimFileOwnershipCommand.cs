using Backbone.Modules.Files.Application.Files.DTOs;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.ClaimFileOwnership;

public class ClaimFileOwnershipCommand : IRequest<ClaimFileOwnershipResponse>
{
    public ClaimFileOwnershipCommand(FileOwnershipTokenDTO ownershipToken, string fileId)
    {
        OwnershipToken = ownershipToken.Value;
        FileId = fileId;
    }

    public string FileId { get; set; }
    public string? OwnershipToken { get; set; }
}

using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.ClaimFileOwnership;

public class ClaimFileOwnershipCommand : IRequest<ClaimFileOwnershipResponse>
{
    public ClaimFileOwnershipCommand(ClaimFileOwnershipRequest ownershipToken, string fileId)
    {
        OwnershipToken = ownershipToken.Values.FirstOrDefault();
        FileId = fileId;
    }

    public string FileId { get; set; }
    public string? OwnershipToken { get; set; }
}

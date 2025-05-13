using Backbone.Modules.Files.Application.Files.DTOs;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.ValidateFileOwnershipToken;

public class ValidateFileOwnershipTokenQuery : IRequest<ValidateFileOwnershipTokenResponse>
{
    public ValidateFileOwnershipTokenQuery(FileOwnershipTokenDTO ownershipToken, string fileId)
    {
        OwnershipToken = ownershipToken.Value;
        FileId = fileId;
    }

    public string FileId { get; }
    public string OwnershipToken { get; }
}

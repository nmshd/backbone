using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.ValidateFileOwnershipToken;

public class ValidateFileOwnershipTokenQuery : IRequest<ValidateFileOwnershipTokenResponse>
{
    public ValidateFileOwnershipTokenQuery(string ownershipToken, string fileId)
    {
        OwnershipToken = ownershipToken;
        FileId = fileId;
    }

    public string FileId { get; }
    public string OwnershipToken { get; }
}
